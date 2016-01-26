#!/usr/bin/env python

import requests
import travispy
import os

def deploy_setup(gh_token, deploy_yml):
    print '------------------------------------------------------------------------------------------------------------------------'
    print "Starting deployment setup. ---------------------------------------------------------------------------------------------"
    print '------------------------------------------------------------------------------------------------------------------------'

    #grab the user from the repo slug.
    user = os.environ["TRAVIS_REPO_SLUG"].split("/")[0]

    #grab the repo name from the repo slug.
    project = os.environ["TRAVIS_REPO_SLUG"].split("/")[1]

    #the url to request to: not that instead of a '/' like in the slug, a '%2F' is required here.
    url = "https://api.travis-ci.org/repo/" + user + "%2F" + project + "/requests"

    #grabs the api_token via travispy (it's there, why not use it?) authenticate with the gh token then grab the token from the headers (see TravisPy class)
    api_token = travispy.TravisPy.github_auth(gh_token)._session.headers['Authorization'].split()[1]

    #the headers required by travis. see: https://docs.travis-ci.com/user/triggering-builds
    headers = {"Content-Type": "application/json",
                "User-Agent": "UnityPackageAssist/0.0.0",
                "Accept": "application/vnd.travis-ci.2+json",
                "Travis-API-Version": "3",
                "Authorization": "token %s" % api_token}

    #the json request. token is here again just to be sure but probably isn't needed.
    requestdict = {"message": "Deployment requested. Rebuilding.",
                    "branch": os.environ["TRAVIS_BRANCH"],
                    "token": api_token,
                    "config": deploy_yml}

    #nested dicts are confusing, this lets me think properly <.<
    json = {"request": requestdict}
    
    response = requests.post(url, headers=headers, json=json)
    print '------------------------------------------------------------------------------------------------------------------------'
    print "Request sent. ----------------------------------------------------------------------------------------------------------"
    print '------------------------------------------------------------------------------------------------------------------------'

    if response.status_code == 202:
        print '------------------------------------------------------------------------------------------------------------------------'
        print "Request accepted by Travis-CI. Rebuilding... ---------------------------------------------------------------------------"
        print '------------------------------------------------------------------------------------------------------------------------'
        exit(0)

    if response.status_code != requests.codes.ok:
        print '------------------------------------------------------------------------------------------------------------------------'
        print "Response status code: %s" % response.status_code
        print '------------------------------------------------------------------------------------------------------------------------'
        print "Response history: %s" % response.history
        print '------------------------------------------------------------------------------------------------------------------------'
        raise response.raise_for_status()



