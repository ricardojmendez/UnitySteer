xbuild ./Net35Essentials/Net35Essentials.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/Net35Essentials/DLLs/
rsync -avv ./Net35Essentials/src/Net35Essentials/bin/Release/ Dependency\ Builds/Net35Essentials/DLLs/

xbuild ./Testity.Unity3D.Events/Testity.Unity3D.Events.sln /p:Configuration=Release /p:Platform="Any CPU"
mkdir -p Dependency\ Builds/Testity.Unity3D.Events/DLLs/
rsync -avv ./Testity.Unity3D.Events/src/Testity.Unity3D.Events.Editor/bin/Release/ Dependency\ Builds/Testity.Unity3D.Events/DLLs/
rsync -avv ./Testity.Unity3D.Events/src/Testity.Unity3D.Events/bin/Release/ Dependency\ Builds/Testity.Unity3D.Events/DLLs/