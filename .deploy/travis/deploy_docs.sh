#! /bin/sh

if [ "$gen_diagrams" == "YES" ]; then
    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "Downloading GraphViz; --------------------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
    curl -o .deploy/dotgraph.pkg http://www.graphviz.org/pub/graphviz/stable/macos/mountainlion/graphviz-2.36.0.pkg

    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "Installing GraphViz; ---------------------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
    if [ "$verbose" == "True" ];
    then
        sudo installer -dumplog -package .deploy/dotgraph.pkg -target /
    else
        sudo installer -package .deploy/dotgraph.pkg -target /
    fi
fi

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Writing options to Doxyfile; -------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"
echo 'OUTPUT_DIRECTORY=./.deploy/docs/output/
OPTIMIZE_OUTPUT_JAVA=YES
GENERATE_LATEX=NO
EXCLUDE=./.deploy/ .travis.yml .deploy.ini ./Project/
RECURSIVE=YES
INPUT=./
DOT_PATH=/usr/local/bin

PROJECT_NAME=$(projectname)
PROJECT_NUMBER=$(docs_version)
PROJECT_BRIEF=$(description)
PROJECT_LOGO=$(logo)
EXTRACT_ALL=$(include_non_documented)
EXTRACT_STATIC=$(include_non_documented)
EXTRACT_PRIVATE=$(include_privates)
EXTRACT_PACKAGE=$(include_privates)
SEARCHENGINE=$(include_search)
GENERATE_TREEVIEW=$(include_nav_panel)
CLASS_DIAGRAMS=$(class_diagrams)
HAVE_DOT=$(gen_diagrams)' >>./.deploy/docs/Doxyfile

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Downloading Doxygen; ---------------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"
curl -o .deploy/doxygen.dmg http://ftp.stack.nl/pub/users/dimitri/Doxygen-1.8.11.dmg

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Installing Doxygen; ----------------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"
sudo hdiutil attach .deploy/doxygen.dmg

sudo ditto /Volumes/Doxygen /Applications/Doxygen

sudo rm -rf /Applications/Doxygen.app

sudo mv /Applications/Doxygen/Doxygen.app /Applications/

sudo rm -r /Applications/Doxygen

sudo hdiutil detach /Volumes/doxygen

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Running Doxygen; -------------------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"
/Applications/Doxygen.app/Contents/Resources/doxygen ./.deploy/docs/Doxyfile

echo "------------------------------------------------------------------------------------------------------------------------"
echo "Pushing docs to Github Pages; ------------------------------------------------------------------------------------------"
echo "------------------------------------------------------------------------------------------------------------------------"

#check if gh-pages already exists, if it doesn't, force it through.
if git ls-remote --exit-code "https://github.com/${TRAVIS_REPO_SLUG}.git" gh-pages = 0; then
    
    #make a new directory to hold the temp repo
    mkdir .deploy/docs/output/temp_git/
    cd .deploy/docs/output/temp_git/ || exit 1
    git init
    
    #random user and email, this is no big deal.
    git config user.name "Travis-CI Doxygen Deployment"
    git config user.email "doxygen@deployment_to.github.pages"
    
    #since we already tested it exists, pull from the gh pages branch and silence it.
    git pull --quiet "https://${GH_TOKEN}@${GH_REF}" gh-pages &> /dev/null
    
    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "Kill everything; -------------------------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "$PWD"
    #kill everything inside except the .git folder.
    find . ! -name "." ! -name ".git" ! -path "./.git/*" -exec rm -r -v {} \;
    cd ../html || exit 1
    
    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "Moving to proper directory; --------------------------------------------------------------------------------------------"
    echo "------------------------------------------------------------------------------------------------------------------------"
    echo "$PWD"
    #copy everything from the doxygen output to the repo.
    find ./* -exec rsync -R -r -v {} ../temp_git/ \;
    cd ../temp_git/ || exit 1
    
    #add everything that changed and commit. -A is used instead of "." for compatibility.
    git add -A
    git commit -m "Deploying to GitHub Pages"
    
    git push --quiet "https://${GH_TOKEN}@${GH_REF}" HEAD:gh-pages &> /dev/null
    
else
    # go to the out directory and create a *new* Git repo
    cd .deploy/docs/output/html || exit 1
    git init

    # inside this git repo we'll pretend to be a new user
    git config user.name "Travis-CI Doxygen Deployment"
    git config user.email "doxygen@deployment_to.github.pages"

    git add -A
    git commit -m "Deploying to GitHub Pages"
.
    git push --force --quiet "https://${GH_TOKEN}@${GH_REF}" master:gh-pages &> /dev/null

fi

#just in case travis doesn't know what to do
cd ../../../.. || exit 1
