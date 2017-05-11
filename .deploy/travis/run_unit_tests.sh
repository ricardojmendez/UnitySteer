#! /bin/sh

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Starting Unit Tests; ---------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"

directory=/Applications/Unity/Unity.app/Contents/MacOS/Unity
if [ ! -d "$directory" ];
then
	echo "\nUnity is missing from: $directory\n"
	sh ./.deploy/travis/unity_install.sh
fi

echo "\nRunning unit tests.\n"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
 -batchmode \
 -nographics \
 -runEditorTests \
 -projectPath ./ \
 -editorTestsResultFile ./testresults.xml
 
exitcode=$?;

echo "\nTest result saved to ./testresults.xml\n\t - results:"
cat ./testresults.xml

if [[ $exitcode != 0 ]]; 
then
	echo "\nUnit tests are FAILED, exit code is $exitcode\n"
	exit $exitcode; 
fi