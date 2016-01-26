#!/usr/bin/env python

import ConfigParser, os

def parse_gh():
    
    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('.travis.ini')
    
    if not config.getboolean('Github', 'enable'):
        return None
    
    prerelease = config.getboolean('Github', 'prerelease')
    if not prerelease:
        conditional_prerelease = config.getboolean('Github', 'conditional_prerelease')
        if conditional_prerelease and ("alpha" in os.environ["TRAVIS_TAG"] or "beta" in os.environ["TRAVIS_TAG"]):
            prerelease = True
    
    draft = config.getboolean('Github', 'draft')
    if not draft:
        conditional_draft = config.getboolean('Github', 'conditional_draft')
        if conditional_draft and ("alpha" in os.environ["TRAVIS_TAG"] or "beta" in os.environ["TRAVIS_TAG"]):
            draft = True
    
    description = config.get('Github', 'description')
    branch = config.get('Github', 'branch')
    
    deploy_gh = {
        "provider": "releases",
        "api_key": os.environ["GH_TOKEN"],
        "target_commitish": os.environ["TRAVIS_COMMIT"],
        "name": os.environ["TRAVIS_TAG"],
        "draft": draft,
        "prerelease": prerelease,
        "skip_cleanup": "true",
        "on": {}
    }
    
    if description:
        deploy_gh["description"] = description
    
    if branch:
        deploy_gh["on"]["branch"] = branch
    else:
        deploy_gh["on"]["all_branches"] = "true"
    
    if os.environ["packagename"]:
        if os.environ["include_version"] == "True":
            deploy_gh["file"] = "./Deploy/%s_%s.zip" % (os.environ["packagename"], os.environ["TRAVIS_TAG"])
        else:
            deploy_gh["file"] = "./Deploy/%s.zip" % os.environ["packagename"]
    else:
        if os.environ["include_version"] == "True":
            deploy_gh["file"] = "./Deploy/%s_%s.zip" % (os.environ["TRAVIS_REPO_SLUG"].split("/")[1], os.environ["TRAVIS_TAG"])
        else:
            deploy_gh["file"] = "./Deploy/%s.zip" % os.environ["TRAVIS_REPO_SLUG"].split("/")[1] #this is the repo name
    
    return deploy_gh


