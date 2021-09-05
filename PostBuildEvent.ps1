[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]

Param
(
    # PARAM1
    [Parameter(Mandatory=$true, ValueFromPipeline=$True, ValueFromPipelineByPropertyName=$True, ValueFromRemainingArguments=$false,
        HelpMessage='Full path to the manifest psd1 file',
        Position=0)]
    [string]
    $ManifestFile,

	# PARAM2
    [Parameter(Mandatory=$true, ValueFromPipeline=$True, ValueFromPipelineByPropertyName=$True, ValueFromRemainingArguments=$false,
        HelpMessage='Full path to the module psm1 file',
        Position=0)]
    [string]
    $ModuleFile
)


Update-ModuleManifest -Path $ManifestFile -ModuleVersion (get-item $ModuleFile).VersionInfo.FileVersion
