#!/usr/bin/env python

import ConfigParser, os

def parse_misc():
    
    config = ConfigParser.RawConfigParser(allow_no_value=True)
    config.read('config.ini')
    
    try: #check if the env already exists, it's better not to mess with existing stuff
        os.environ["verbose"]
        os.environ["always_run"]
        os.environ["include_version"]
        os.environ["packagename"]
    except KeyError:
        os.environ["verbose"] = str(config.getboolean('Misc', 'verbose'))
        os.environ["always_run"] = str(config.getboolean('Misc', 'always_run'))
        os.environ["packagename"] = config.get('Misc', 'packagename')
        if os.environ["TRAVIS_TAG"].strip():
            os.environ["include_version"] = str(config.getboolean('Misc', 'include_version')) #version is pulled from tag
        else:
            os.environ["include_version"] = "False" #so no sense having it when there is no tag.
    else:
        print "Something in [Misc] already exists as an env variable. Change it to something else."
        exit(1)
