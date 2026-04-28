$testDir = "C:\Users\Bhrenno Borges\Desktop\winfimTeste"

Write-Host "=========================================="
Write-Host "  WinFIM Guard - Simulador de Eventos"
Write-Host "=========================================="
Write-Host ""

if (-not (Test-Path $testDir)) {
    New-Item -ItemType Directory -Path $testDir | Out-Null
    Write-Host "[+] Diretório de testes criado: $testDir"
} else {
    Write-Host "[*] Diretório de testes já existe: $testDir"
}

Write-Host ""
Write-Host "Iniciando simulação de eventos em 3 segundos..."
Write-Host "(Certifique-se de que a aba 'Eventos' do WinFIM Guard está aberta)"
Start-Sleep -Seconds 3
Write-Host ""

# 1. Criação de executável (Deve gerar HIGH)
Write-Host "[1/5] Simulando criação de executável (.exe)... -> Esperado: Severidade High"
$exePath = Join-Path $testDir "ransomware_simulado.exe"
Set-Content -Path $exePath -Value "MzkwMzkwMzkw..." # Falso binario
Start-Sleep -Seconds 2

# 2. Criação de script (Deve gerar HIGH)
Write-Host "[2/5] Simulando criação de script PowerShell (.ps1)... -> Esperado: Severidade High"
$ps1Path = Join-Path $testDir "backdoor.ps1"
Set-Content -Path $ps1Path -Value "Invoke-WebRequest -Uri 'http://hacker.com'"
Start-Sleep -Seconds 2

# 3. Criação de um PDF e Exclusão dele (Criação = MEDIUM, Exclusão = HIGH)
Write-Host "[3/5] Criando documento sigiloso (.pdf)... -> Esperado: Severidade Medium"
$pdfPath = Join-Path $testDir "folha_pagamento.pdf"
Set-Content -Path $pdfPath -Value "Salários: ..."
Start-Sleep -Seconds 2

Write-Host "[4/5] Simulando exclusão de documento crítico (.pdf)... -> Esperado: Severidade High"
Remove-Item -Path $pdfPath
Start-Sleep -Seconds 2

# 5. Modificação de arquivo normal (Deve gerar MEDIUM)
Write-Host "[5/5] Criando e modificando arquivo comum (.txt)... -> Esperado: Severidade Medium"
$txtPath = Join-Path $testDir "anotacoes.txt"
Set-Content -Path $txtPath -Value "Comprar leite"
Start-Sleep -Seconds 1
Add-Content -Path $txtPath -Value "`nComprar pão"
Start-Sleep -Seconds 2

Write-Host ""
Write-Host "=========================================="
Write-Host "Simulação concluída com sucesso!"
Write-Host "Vá até a aba de Eventos no WinFIM Guard e confira os alertas gerados."
Write-Host "=========================================="
