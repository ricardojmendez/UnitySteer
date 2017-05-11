#! /bin/sh

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Starting Unit Tests; ---------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"

directory=/Unity/Unity.app/Contents/MacOS/Unity
if [ ! -d "$directory" ]; then
then
	echo "\nUnity is missing from: $directory\n"
	sh ./.deploy/travis/unity_install.sh
fi

echo "\nRunning unit tests.\n"
/Unity/Unity.app/Contents/MacOS/Unity -runEditorTests -projectPath ./ -editorTestsResultFile ./testresults.xml

echo "\nTest result saved: ./testresults.xml\n"
cat ./testresults.xml
