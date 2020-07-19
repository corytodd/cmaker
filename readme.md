# CMaker

A tool for generating C++ CMake projects

## Why?

CMake is as simple or complex as you need it to be. When you have docs, testing, and multiple
projects, version info, and compatibility nuances, it takes a few files to get started. 

1) I like to keep my CMake projects standardized
2) I like to start with the proper folder structure
3) I like to have GTest ready to go


If you agree with those points this CMaker might be useful for you.

## Building 

This is a dotnet core tool because it provides the most portable cross-platform file system API that can be easily
compiled into a single application.

    dotnet build -c Release 
    
and you're done. If you want a publish build, see `release.bat` and adjust for your platform.

## Templates

CMaker.Core contains a template called standard.template which is a sanitized and zipped cmake project that I 
like to base new projects on. There are two types of variables in the template.

1) File/Directory templates use CMAKE_LIB_NAME in the file/directory as a placeholder for the project name. 
2) CMaker variables are in the format %%CMAKER_XXX%% and exist inside certain files. 

### Supported variables    

- CMAKER_PROJECT_NAME the user-provided project name

- CMAKER_ROOT_NAMESPACE root namespace for C++ classes

- CMAKER_NORMALIZED_NAME he user-provided project name with spaces removed

- CMAKER_NORMALIZED_NAME_LOWER he user-provided project name with spaces removed in lower case 

- CMAKER_DATE Current date in format mm/dd/yyyy
    
- CMAKER_CLI_NAME Normalized project name with "_cli" suffix


## References

Lots of time and effort have gone into making the best possible cmake structure. Here are the projects and 
references that helped shape the standard template:

- https://github.com/rpavlik/cmake-modules

- https://github.com/robotology/how-to-export-cpp-library

- https://cliutils.gitlab.io/modern-cmake/chapters/testing/googletest.html

- https://github.com/halex2005/CMakeHelpers/blob/master/generate_product_version.cmake

- Other inline references are marked in their respective files

