# CSharpFilesMerger
A command line utility to merge multiple C# file (*.cs) into one

* Scan a directory or give a list of files
* Multiple way to manage `using`
* Merge `partial class` in one class
* Ignore `*\obj\*`, `*\Properties\*` and `*\Merge.cs`

## Parameters
By default the application search for *.cs files in the working directory of the application.\
You can specify an other directory as first argument.\
It can be an absolute path or a relative path from working directory.

### Syntax:
```CSharpFilesMerger [workingDirectory] [OPTIONS]```

### Options:  
|Option           | Description |
|-----------------|-------------|
| `-h, --help`      | Show this help text<br />(is exclusive with others options except `-w`)|
| `-v, --version`   | Show the version of the application<br />(is exclusive with others options except `-w`)|
| `-r, --recursive` | Search for *.cs file recursively from working directory or specified directory.<br />Do not work with `-f, --files`|
| `-f, --files`     | To specify the list of files to merge. Only these files will be merged.<br />Each file must be separate by a semicolon ";". <br />Each file can be specify as absolute path or relative to working directory or specified directory.<br /><br />**Example** : `-f "a.cs;..\b.cs;C:\myDirectory\c.cs"`<br /><br />*Remark : When this option is active files are merged in the order of the list.*|
| `-o, --out`       | To specify the fileName of the output file where *.cs files are merged.<br />Can be specify as absolute path or relative to working directory or specified directory.<br /><br />**Example** : `-o "MyOutputFile.cs"`<br />**Example** : `-o "C:\myDirectory\MyOutputFile.cs"`<br /><br />**Default value** is equivalent to : `-o "Merged.cs"`|
| `-u, --usings`    | To specify where to put usings (globally vs in namespaces).<br />Must be followed by one of these 3 options :<br /><br /><ul><li>**DoNotMove** : keep the found "using" globally if it is defined outside of any namespace or in the specific namespace if found in a namespace.</li><li>**Global**    : Force all "using" to be defined globally in the merged result.</li><li>**Namespace** : Force all "using" to be defined in namespaces in the merged result.</li></ul>**Default value** is equivalent to : `-u DoNotMove`<br /><br />*Remark : All usings are ordered and print only one time by usings location.*|
| `-s, --start`     | To open the result file at the end.<br />By default try to open the file with the default program for *.cs files.<br />An other program can be optionally specify with the path of the program<br /><br />**Example** : `-s "C:\Program Files\MyEditor\MyEditor.exe"`|
| `-w, --wait`      | Ask the user to press a key to exit at the end. |
