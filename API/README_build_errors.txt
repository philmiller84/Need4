If protos are not generating CS files, check the project file to see if they are listed explicitly with a Remove in the name. 

There should only be a wildcard include for the proto folder, nothing specified other than that.

If there is a file specified, remove that item group and reload and rebuild.
