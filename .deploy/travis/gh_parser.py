#!/usr/bin/env python

import ConfigParser, os

def parse_gh():
    
    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('.deploy.ini')
    
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
    
    title = config.get('Github', 'title')
    description = config.get('Github', 'description')
    branch = config.get('Github', 'branch')
    packagename = config.get('Github', 'packagename')
    include_version = config.getboolean('Github', 'include_version')
    
    deploy_gh = {
        "provider": "releases",
        "api_key": os.environ["GH_TOKEN"],
        "target_commitish": os.environ["TRAVIS_COMMIT"],
        "draft": draft,
        "prerelease": prerelease,
        "skip_cleanup": "true",
        "on": {}
    }
    
    if title:
        deploy_gh["name"] = title
    else:
        deploy_gh["name"] = os.environ["TRAVIS_TAG"]
    
    if description:
        deploy_gh["description"] = description
    
    if branch:
        deploy_gh["on"]["branch"] = branch
    else:
        deploy_gh["on"]["all_branches"] = "true"
    
    if packagename:
        if include_version:
            deploy_gh["file"] = "./Deploy/%s_%s.zip" % (packagename, os.environ["TRAVIS_TAG"])
        else:
            deploy_gh["file"] = "./Deploy/%s.zip" % packagename
    else:
        if include_version:
            deploy_gh["file"] = "./Deploy/%s_%s.zip" % (os.environ["TRAVIS_REPO_SLUG"].split("/")[1], os.environ["TRAVIS_TAG"])
        else:
            deploy_gh["file"] = "./Deploy/%s.zip" % os.environ["TRAVIS_REPO_SLUG"].split("/")[1] #this is the repo name
    
    return deploy_gh

def parse_gh_options():
    
    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('.deploy.ini')
    
    options = []
    
    packagename = config.get('Github', 'packagename')
    
    include_version = config.getboolean('Github', 'include_version')
    
    if packagename:
        options.append("packagename=\"%s\"" % packagename)
    else:
        options.append("packagename=%s" % os.environ["TRAVIS_REPO_SLUG"].split("/")[1]) #repo name!
    
    options.append("gh_version=%s" % include_version)
    
    return options
