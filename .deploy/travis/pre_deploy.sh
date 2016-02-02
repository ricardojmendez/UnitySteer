#! /bin/sh

if [ "$packagename" == "" ]; then
    project="${TRAVIS_REPO_SLUG##*/}"
else
    project="$packagename"
fi
if [ "$gh_version" == "True" ]; then
    package="$project"_"$TRAVIS_TAG".unitypackage
    zip="$project"_"$TRAVIS_TAG".zip
else
    package=$project.unitypackage
    zip="$project".zip
fi

if [ "$project" == "UnityDeployTools" ]; then
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Preparing directories; -------------------------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  mkdir ./Temp
  mkdir ./Deploy
  
  #grab everything inside .deploy/ except the files created during the build. Also grab the readme and license.
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Moving relevant files to Temp/ directory; ------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  mv ./.deploy ./Temp/.deploy
  rm ./Temp/.deploy/*.pkg
  rm ./Temp/.deploy/*.dmg
  rm -rf ./Temp/.deploy/docs/output/
  if [ "$verbose" == "True" ];
  then
    rm ./Temp/.deploy/*.log
  fi
  mv README.rst ./Temp/.deploy/README.rst
  mv LICENSE ./Temp/.deploy/LICENSE
  mv .deploy.ini ./Temp/.deploy.ini
  
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Writing new yml file; --------------------------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo 'language: objective-c

install:
  - sh ./.deploy/travis/py_set_up.sh

script:
  - python ./.deploy/travis/main_parser.py' >./Temp/.travis.yml
  if [ "$verbose" == "True" ];
  then
    cat ./Temp/.travis.yml
  fi

  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Writing new config file; -----------------------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo $'[Misc]
#if set to true, all logs from commands will be shown. Default is false.
verbose=false

#if set to true, Travis will always try to build the package/asset, even when there isn\'t a tag. Default is true. 
always_run=true

[Docs]
#if set to true, will enable generating docs and deploying them to github pages. Default is true.
enable=true

#if you want to generate documentation only from a specific branch. Default is master.
branch=master

#if you want to name your project something other than the repo name, fill in:
projectname=

#if you want the short description to be something other than your repo description, fill in:
description=

#if you wish your project to have a logo, fill in the relative path to the image.
#e.g. if you store it in the .deploy folder, fill in this: ./.deploy/my_logo.png
logo=

#if set to true, will include the tag as the documentation version. Default is false.
include_version=false

#if set to true, will include all code even if not documented. Default is true.
include_non_documented=true

#if set to true, will include private members in the documentation. Default is true.
include_privates=true

#if set to true, will include a sidebar with a navigation panel. Default is true.
include_nav_panel=true

#if set to true, will include a search function in each page. Default is true.
include_search=true

#if set to true, will generate class hierarchy diagrams. Default is true.
gen_diagrams=true

[Github]
#if set to true, will enable deployment to github if possible. Default is true.
enable=true

#if set to true, tag will be included after the package name (e.g. UnityDeployTools_v1.1). Default is true.
include_version=true

#if you want to name the deploy zip file something other than your repo name:
packagename=

#if set to true, tags with "alpha" or "beta" in their name will be set to prerelease. Default is true.
conditional_prerelease=true

#if set to true, tags with "alpha" or "beta" in their name will be deployed as draft. Default is true.
conditional_draft=true

#if set to true, releases will always be set to prerelease. 
#Overrides conditional_prerelease if true. Default is false.
prerelease=false

#if set to true, releases will always be deployed as a draft. 
#Overrides conditional_draft if true. Default is false.
draft=false

#if you want to name the release something other than the tag, fill in:
title=

#if you want to add something (don\'t forget this should be in github markdown) 
#to the release description: 
description=

#if you want to deploy only from a specific branch:
branch=

[AssetStore]
#not supported YET' >./Temp/.deploy.ini
  if [ "$verbose" == "True" ];
  then
    cat ./Temp/.deploy.ini
  fi
  
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Compressing relevant files to Deploy/ directory; -----------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  cd Temp/ || exit 1
  zip -r -X "$zip" .
  cd ..
  mv ./Temp/"$zip" ./Deploy/"$zip"
  
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Checking compression was successful; -----------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  
  file=./Deploy/$zip
  
  if [ -e "$file" ];
  then
    echo "------------------------------------------------------------------------------------------------------------------------"
	echo "Package compressed successfully: $file"
    echo "------------------------------------------------------------------------------------------------------------------------"
	exit 0
  else
    echo "------------------------------------------------------------------------------------------------------------------------"
	echo "Package not compressed. Aborting.---------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
	exit 1
  fi
else
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Compressing package to Deploy/ directory; ------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  mkdir ./Deploy
  
  cd Project/ || exit 1
  zip -r -X "$zip" "$package"
  cd ..
  
  mv ./Project/"$zip" ./Deploy/"$zip"
   
  echo "------------------------------------------------------------------------------------------------------------------------"
  echo "Checking compression was successful; -----------------------------------------------------------------------------------"
  echo "------------------------------------------------------------------------------------------------------------------------"
  
  file=./Deploy/$zip
   
  if [ -e "$file" ];
  then
    echo "------------------------------------------------------------------------------------------------------------------------"
	echo "Package compressed successfully: $file"
    echo "------------------------------------------------------------------------------------------------------------------------"
	exit 0
  else
    echo "------------------------------------------------------------------------------------------------------------------------"
	echo "Package not compressed. Aborting. --------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
	exit 1
  fi
fi
