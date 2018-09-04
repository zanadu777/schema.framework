param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

Copy 'PsSchema.dll' '.\PsSchema' -Force -Verbose
Copy 'PsSchema.pdb' '.\PsSchema' -Force -Verbose

Copy 'Errata.Collections.dll' '.\PsSchema' -Force -Verbose
Copy 'Errata.Collections.pdb' '.\PsSchema' -Force -Verbose


Copy 'Schema.Common.dll' '.\PsSchema' -Force -Verbose
Copy 'Schema.Common.pdb' '.\PsSchema' -Force -Verbose

Copy 'Schema.SqlServer.dll' '.\PsSchema' -Force -Verbose
Copy 'Schema.SqlServer.pdb' '.\PsSchema' -Force -Verbose