# push.ps1 - Push this repo to GitHub
# Usage: .\push.ps1            (normal push)
#        .\push.ps1 --tags     (push tags too)
#
# Why this script: this machine rewrites github.com URLs to gh-proxy.com
# (git config url.<base>.insteadOf), and the proxy is fetch-only. So we
# temporarily disable the rewrite, push straight to github.com, then put
# it back.

$repo = $PSScriptRoot
$rewriteKey = 'url.https://gh-proxy.com/https://github.com/.insteadOf'
$rewriteVal = 'https://github.com/'
$realRemote = 'https://github.com/XHszDHM/rimworld-mods.git'
$proxyRemote = 'https://gh-proxy.com/https://github.com/XHszDHM/rimworld-mods.git'

$existing = git config --global --get $rewriteKey 2>$null
if ($existing) { git config --global --unset $rewriteKey 2>$null }

git -C $repo remote set-url origin $realRemote
$env:GIT_SSL_NO_VERIFY = 'true'
git -C $repo push @args
$code = $LASTEXITCODE
Remove-Item Env:\GIT_SSL_NO_VERIFY -ErrorAction SilentlyContinue

if ($existing) { git config --global $rewriteKey $rewriteVal }
git -C $repo remote set-url origin $proxyRemote

if ($code -eq 0) { Write-Host 'Push OK' -ForegroundColor Green }
else { Write-Host ('Push failed, exit code ' + $code) -ForegroundColor Red }
exit $code
