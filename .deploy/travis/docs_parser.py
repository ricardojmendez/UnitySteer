#!/usr/bin/env python

import ConfigParser, os, requests

def parse_docs():

    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('.deploy.ini')
    
    if not config.getboolean('Docs', 'enable'):
        return None
    
    branch = config.get('Docs', 'branch')
    
    deploy_docs = {
        "provider": "script",
        "script": "./.deploy/travis/deploy_docs.sh",
        "skip_cleanup": "true",
        "on": {}
    }
    
    if branch:
        if branch != os.environ["TRAVIS_BRANCH"]:
            return None
        deploy_docs["on"]["branch"] = branch
    else:
        deploy_docs["on"]["all_branches"] = "true"
    
    #return deploy_docs
    return ["sh ./.deploy/travis/deploy_docs.sh"]

def get_github_description():
    
    headers = {
        "Accept": "application/vnd.github.v3+json",
        "Authorization": "token %s" % os.environ["GH_TOKEN"],
        "User-Agent": os.environ["TRAVIS_REPO_SLUG"].split("/")[0]
    }
    
    url = "https://api.github.com/repos/%s" % os.environ["TRAVIS_REPO_SLUG"]
    
    response = requests.get(url, headers=headers)
    
    if response.status_code != requests.codes.ok:
        print '------------------------------------------------------------------------------------------------------------------------'
        print "Response status code: %s" % response.status_code
        print '------------------------------------------------------------------------------------------------------------------------'
        print "Response history: %s" % response.history
        print '------------------------------------------------------------------------------------------------------------------------'
        raise response.raise_for_status()

    return response.json()["description"]
    
def parse_docs_options():
    
    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('.deploy.ini')
    
    options = []
    
    projectname = config.get('Docs', 'projectname')
    description = config.get('Docs', 'description')
    logo = config.get('Docs', 'logo')
    
    include_version = config.getboolean('Docs', 'include_version')
    include_non_documented = config.getboolean('Docs', 'include_non_documented')
    include_privates = config.getboolean('Docs', 'include_privates')
    include_nav_panel = config.getboolean('Docs', 'include_nav_panel')
    include_search = config.getboolean('Docs', 'include_search')
    gen_diagrams = config.getboolean('Docs', 'gen_diagrams')
    
    if projectname:
        options.append("projectname=\"%s\"" % projectname)
    else:
        options.append("projectname=%s" % os.environ["TRAVIS_REPO_SLUG"].split("/")[1]) #repo name!
    
    if description:
        options.append("description=\"%s\"" % description)
    else:
        options.append("description=\"%s\"" % get_github_description())
    
    if logo:
        options.append("logo=\"%s\"" % logo)
    else:
        options.append("logo=") #I hope this doesn't throw an error.
    
    if include_version:
        options.append("docs_version=%s" % os.environ["TRAVIS_TAG"])
    else:
        options.append("docs_version=")
    
    if include_non_documented:
        options.append("include_non_documented=YES")
    else:
        options.append("include_non_documented=NO")
    
    if include_privates:
        options.append("include_privates=YES")
    else:
        options.append("include_privates=NO")
    
    if include_nav_panel:
        options.append("include_nav_panel=YES")
    else:
        options.append("include_nav_panel=NO")
    
    if include_search:
        options.append("include_search=YES")
    else:
        options.append("include_search=NO")
    
    if gen_diagrams:
        options.append("gen_diagrams=YES")
        options.append("class_diagrams=NO") #this is just the opposite of the dot graphs - I set it to the opposite here just because it's easier than at deployment
    else:
        options.append("gen_diagrams=NO")
        options.append("class_diagrams=YES")
    
    options.append("GH_REF=github.com/%s.git" % os.environ["TRAVIS_REPO_SLUG"]) #this is needed for the push to gh-pages
    
    return options

