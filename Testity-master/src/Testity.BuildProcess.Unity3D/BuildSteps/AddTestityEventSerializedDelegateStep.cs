using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class AddTestityEventSerializedDelegateStep : ITestityBuildStep
	{
		private readonly ActionTypeRelationalMapper actionMapper;

		private readonly ITypeMemberParser typeParser;

		private readonly TestityGenericEventTracker eventTracker;

		public AddTestityEventSerializedDelegateStep(ActionTypeRelationalMapper mapper, TestityGenericEventTracker tracker, ITypeMemberParser parser)
		{
			actionMapper = mapper;
			typeParser = parser;
			eventTracker = tracker;
        }

		public void Process(IClassBuilder builder, Type typeToParse)
		{
			//This is so inefficient. We do this so often, thankfully it's technically cached.
			//Anyway, we need the member types again.

			IEnumerable<MemberInfo> parsedInfos = typeParser.Parse(System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Field, typeToParse);

			//Should now be a collection of only Action fields
			IEnumerable<MemberInfo> actionInfoTypes = parsedInfos.Where(x => actionMapper.isActionType(x.Type()));

			foreach(MemberInfo mi in actionInfoTypes)
			{
				//this class can't handle generic events
				if (actionMapper.isGenericActionType(mi.Type()))
				{
					//get a new class name that is shared across all events of the same sig
					string newClassName = actionMapper.GetSharedGenericEventTypeName(mi.Type().GetGenericArguments());
					eventTracker.Register(newClassName, mi.Type().GetGenericArguments()); //register info about the generic event to be tracked

					//we can add the field to the class now
					builder.AddClassField(new UnitySerializedFieldImplementationProvider(mi.Name, newClassName, new Common.Unity3D.WiredToAttribute(mi.MemberType, mi.Name, mi.Type())));
				}
				else
					//action mapper can resolve Action/TestityEvent types
					//Non-generic events require no messing with
					builder.AddClassField(new UnitySerializedFieldImplementationProvider(mi.Name, actionMapper.ResolveMappedType(mi.Type()), new Common.Unity3D.WiredToAttribute(mi.MemberType, mi.Name, mi.Type())));
			}
		}
	}
}
