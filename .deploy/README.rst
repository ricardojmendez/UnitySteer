.. |travisbadge| image:: https://travis-ci.org/GandaG/unity-deploy-tools.svg?branch=master
    :target: https://travis-ci.org/GandaG/unity-deploy-tools

.. |nbsp| unicode:: 0xA0 
   :trim:

####################################################################################
Unity Deployment Tools |nbsp| |nbsp| |nbsp| |travisbadge|
####################################################################################
*Continuous deployment tools for open-source* `Unity3D <https://unity3d.com/>`_ *packages and assets.*

This repository offers a quick way to setup open source unity packages. It features:

- a self-documented and easy to use .ini file to configure all the options;
- automated package/asset exporting from unity;
- automated deployment to Github Releases;
- automated creation and deployment of documentation to Github Pages;
- automated deployment to Unity's Asset Store. [TODO]


************
Installation
************
1. Download the `latest stable release <https://github.com/GandaG/unitypackage-ci/releases/latest>`_;

2. Unpack the files into your repository's root directory;

3. Go to `Travis-CI <https://travis-ci.org/>`_ an sign in with Github;

4. Enable your repository in your account page;

5. Open :code:`.deploy.ini` with a text editor (like Notepad++) and modify the options under the :code:`[Misc]` section as per your preferences.

6. For additional features check the subsections below;

7. Simply push a commit and let travis do all the work for you!

*******************
Additional Features
*******************
In order to use any of the features below you need to grab an OAuth token from Github with special permissions. Take these steps:

1. Sign in to Github;

2. Go to your `Settings <https://github.com/settings/>`_;

3. Go to `Personal Access Tokens <https://github.com/settings/tokens>`_;

4. Click on :code:`Generate new token`;

5. Enter your password;

6. Give the token a good description. Mine is :code:`Travis-CI Deploy`;

7. Give the token these permissions:

   - gist;
   - read:org; 
   - repo; 
   - user:email;
   - write:repo_hook

8. Click on :code:`Generate token`;

9. The new token should now appear. NEVER give or show this token to anyone, not even Travis (the encryption process later on does not send the token to Travis, the entire process is local);

10. Temporarily store your token somewhere local and safe;

11. Go to `Travis-CI <https://travis-ci.org/>`_ and sign in with Github;

12. Go to your repository settings;

13. In the Environment Variables section, write "GH_TOKEN" (without the quotes) in the :code:`Name` field and paste the token in the :code:`Value` field;

14. Make sure to leave :code:`Display value in build log` as :code:`OFF` and click :code:`Add`;

15. It's now safe to delete and forget about that token from before! Only use the secure (encrypted) version from now on.

Github Releases
""""""""""""""""""
Simply open your :code:`.deploy.ini` file with a text editor (like Notepad++) and modify the :code:`[Github]` 
section as per your preferences. That's it, really.

Code Documentation
""""""""""""""""""
Documentation for all your code is generated with `Doxygen <http://www.stack.nl/~dimitri/doxygen/index.html/>`_.
Example documentation is provided at the `Github Pages <https://gandag.github.io/unity-deploy-tools/>`_ of this repository, 
example script taken from Microsoft's `XML Documentation Example <https://msdn.microsoft.com/en-us/library/aa288481(v=vs.71).aspx>`_.

Basic Usage
'''''''''''
Documentation generation is enabled by default. Open your :code:`.deploy.ini` file with a text editor (like Notepad++) 
and modify the :code:`[Docs]` section as per your preferences. That's pretty much it, for more things to do with Doxygen check below.

Advanced/Expert
'''''''''''''''
Doxygen must be (for the sheer amount of options) the best documented software I've ever seen. 

If you want to customize your docs more than the basic options presented in the :code:`.deploy.ini` file 
(these are the same presented in the basic panel of the doxywizard) head over to :code:`.deploy/docs/` and 
open the :code:`Doxyfile`. This is the default doxygen configuration file and every option is very well 
documented. There are bajillions of them, have fun!

Please note that some options are hardcoded (see :code:`.deploy/travis/deploy_docs.sh`).

*****************
Upcoming Features
*****************
- Automated package deployment to `Unity <https://unity3d.com/>`_'s Asset Store.
- Automated compiling and distribution as a .dll file.
