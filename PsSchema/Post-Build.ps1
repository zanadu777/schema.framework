param(
	[Parameter()] $ProjectName,
	[Parameter()] $ConfigurationName,
	[Parameter()] $TargetDir
)

Copy 'PsSchema.dll' '.\PsSchema' -Force -Verbose
