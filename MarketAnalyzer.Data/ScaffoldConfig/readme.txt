------------------------------------------------------------README-------------------------------------------------------------------------------------
*******************************************************************************************************************************************************
Hows:
++++++++++++++++++++++++++++++++++++++++++++++++
+ How to use dotnet core scaffolding command?  +
+ How to debug it?                             +
+ How to add custom behaviour				   +
++++++++++++++++++++++++++++++++++++++++++++++++

a) Scaffolding

Go to the cmd line, go to the BlockBase.Web.Data folder, and run the following command:

dotnet ef dbcontext scaffold "connectionstring" Microsoft.EntityFrameworkCore.SqlServer --force


Notes:

--force to overwrite

b)  Debugging

for debugging purposes of the scaffolding procedures use ScaffoldDebugFileWriter to write a user-defined log to a file inside the Debugging folder.

ex : 

	var debugger = new ScaffoldDebugFileWriter();
	ScaffoldDebugFileWriter(stringToWrite);

c) Custom Scaffolding Behaviour

The beahviour of the scaffolding can be altered by modifying either: 

	- CustomEntityTypeGenerator.cs
	- CustomDbContextGenerator.cs

------------------------------------------------------------------------------------------------------------------------------------------------------
******************************************************************************************************************************************************
