<#
This script will run on debug.
It will load in a PowerShell command shell and import the module developed in the project. To end debug, exit this shell.
#>

# Write a reminder on how to end debugging.
$message = "| Exit this shell to end the debug session! |"
$line = "-" * $message.Length
$color = "Cyan"
Write-Host -ForegroundColor $color $line
Write-Host -ForegroundColor $color $message
Write-Host -ForegroundColor $color $line
Write-Host

# Load the module.
$env:PSModulePath = (Resolve-Path .).Path + ";" + $env:PSModulePath
Import-Module 'PsSchema' -Verbose

# Happy debugging :-)
$constring = "Data Source=192.168.1.12;Initial Catalog=P;Persist Security Info=True;User ID=sa;Password=Iscandar2199"
get-schema   -ConnectionString $constring  -Output Name

