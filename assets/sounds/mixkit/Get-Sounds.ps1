Get-Content .\readme.md `
| Where-Object { $_ -imatch '^\* .* - .*\.(wav|mp3)$' } `
| ForEach-Object {
     [string] $fileName, [string] $url = $_.Substring(2) -split '\s*-\s*'

     [string] $extension = $url -split '\.' `
     | Select-Object -Last 1

     Invoke-WebRequest -Uri $url -OutFile "${fileName}.$extension"
}