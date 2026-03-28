param(
    [string]$RepoUrl
)

Write-Host "=== INICIALIZANDO REPOSITORIO GIT PARA PROJETO C# ===" -ForegroundColor Cyan

if (-not (Test-Path ".git")) {
    git init
    Write-Host "Git inicializado." -ForegroundColor Green
}
else {
    Write-Host "Repositorio Git ja existe nesta pasta." -ForegroundColor Yellow
}

# Cria .gitignore padrao para Visual Studio / C#
$gitignoreContent = @"
## Build
bin/
obj/

## Visual Studio
.vs/
*.user
*.suo
*.userosscache
*.sln.docstates

## VS Code / Rider
.vscode/
.idea/

## Logs
*.log

## NuGet
packages/
*.nupkg

## Publish
publish/

## Testes
TestResults/

## Arquivos temporarios
*.tmp
*.temp

## Windows
Thumbs.db
Desktop.ini

## Arquivos de configuracao sensiveis
appsettings.Development.json
appsettings.Local.json
.env
"@

Set-Content -Path ".gitignore" -Value $gitignoreContent -Encoding UTF8
Write-Host ".gitignore criado/atualizado." -ForegroundColor Green

# Remove cache de arquivos indevidos ja adicionados anteriormente
git rm -r --cached . 2>$null

# Adiciona tudo respeitando .gitignore
git add -A

# Verifica se ha algo para commit
$status = git status --porcelain
if ([string]::IsNullOrWhiteSpace($status)) {
    Write-Host "Nada para commit." -ForegroundColor Yellow
}
else {
    git commit -m "Initial commit"
    Write-Host "Commit criado." -ForegroundColor Green
}

# Ajusta branch principal
git branch -M main
Write-Host "Branch definida como main." -ForegroundColor Green

# Configura remoto, se URL informada
if (-not [string]::IsNullOrWhiteSpace($RepoUrl)) {
    $remoteExists = git remote | Select-String "^origin$"

    if ($remoteExists) {
        git remote set-url origin $RepoUrl
        Write-Host "Remote origin atualizado para $RepoUrl" -ForegroundColor Green
    }
    else {
        git remote add origin $RepoUrl
        Write-Host "Remote origin criado para $RepoUrl" -ForegroundColor Green
    }

    git push -u origin main
    Write-Host "Projeto enviado para a nuvem." -ForegroundColor Green
}
else {
    Write-Host "Nenhuma URL de repositorio informada. Git local preparado, mas sem push." -ForegroundColor Yellow
}

Write-Host "=== FINALIZADO ===" -ForegroundColor Cyan