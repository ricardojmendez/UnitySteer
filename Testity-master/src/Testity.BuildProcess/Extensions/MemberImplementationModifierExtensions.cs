using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess
{
	public static class MemberImplementationModifierExtensions
	{
		/// <summary>
		/// Converts <paramref name="modifiers"/> into a collection of Rosyln <see cref="SyntaxKind"/> enum values.
		/// </summary>
		/// <param name="modifiers">Valid <see cref="MemberImplementationModifier"/> to convert.</param>
		/// <returns>A collection that represents the transform</returns>
		public static IEnumerable<SyntaxKind> ToSyntaxKind(this MemberImplementationModifier modifiers)
		{
			//Select all the values that have the flag
			return (Enum.GetValues(typeof(MemberImplementationModifier)) as IEnumerable<MemberImplementationModifier>)
				.Where(m => modifiers.HasFlag(m))
				.Select(m => ConvertToSyntaxKind(m));
		}

		private static SyntaxKind ConvertToSyntaxKind(MemberImplementationModifier mod)
		{
			switch (mod)
			{
				case MemberImplementationModifier.Private:
					return SyntaxKind.PrivateKeyword;
				case MemberImplementationModifier.Public:
					return SyntaxKind.PublicKeyword;
				case MemberImplementationModifier.Virtual:
					return SyntaxKind.VirtualKeyword;
				case MemberImplementationModifier.Override:
					return SyntaxKind.OverrideKeyword;
				case MemberImplementationModifier.Static:
					return SyntaxKind.StaticKeyword;
				case MemberImplementationModifier.Sealed:
					return SyntaxKind.SealedKeyword;
				case MemberImplementationModifier.Protected:
					return SyntaxKind.ProtectedKeyword;
				default:
					throw new ArgumentException(nameof(MemberImplementationModifier) + " arg is in an invalid state. Could not handle Value: " + mod.ToString(), nameof(mod));
			}
		}
	}
}
