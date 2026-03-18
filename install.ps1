[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$host.UI.RawUI.WindowTitle = 'Ascension Adjuster'

Write-Host '============================================'
Write-Host '  Ascension Adjuster  天梯难度调整器'
Write-Host '  模组安装程序'
Write-Host '============================================'
Write-Host ''

$src = $PSScriptRoot
$dll = Join-Path $src 'AscensionAdjuster.dll'
$json = Join-Path $src 'AscensionAdjuster.json'

if (-not (Test-Path $dll)) {
    Write-Host '[错误] 未找到 AscensionAdjuster.dll' -ForegroundColor Red
    Write-Host '请确保本脚本与模组文件在同一文件夹中。'
    exit 1
}
if (-not (Test-Path $json)) {
    Write-Host '[错误] 未找到 AscensionAdjuster.json' -ForegroundColor Red
    Write-Host '请确保本脚本与模组文件在同一文件夹中。'
    exit 1
}

Write-Host '正在搜索「杀戮尖塔 2」的安装目录，请稍候...'
Write-Host ''

# Read Steam install path from Windows registry
$sp = $null
try { $sp = (Get-ItemProperty 'HKLM:\SOFTWARE\WOW6432Node\Valve\Steam' -EA Stop).InstallPath } catch {}
if (-not $sp) {
    try { $sp = (Get-ItemProperty 'HKCU:\SOFTWARE\Valve\Steam' -EA Stop).SteamPath } catch {}
}

# Parse libraryfolders.vdf to find all Steam library paths
$gp = $null
if ($sp) {
    $vdf = Join-Path $sp 'steamapps\libraryfolders.vdf'
    if (Test-Path $vdf) {
        foreach ($line in Get-Content $vdf) {
            if ($line -match '"path"\s+"([^"]+)"') {
                $p = $Matches[1].Replace('\\', '\')
                $c = Join-Path $p 'steamapps\common\Slay the Spire 2'
                if (Test-Path $c) { $gp = $c; break }
            }
        }
    }
    if (-not $gp) {
        $c = Join-Path $sp 'steamapps\common\Slay the Spire 2'
        if (Test-Path $c) { $gp = $c }
    }
}

if ($gp) {
    Write-Host "找到游戏目录：$gp" -ForegroundColor Green
    Write-Host ''

    $dest = Join-Path $gp 'mods\AscensionAdjuster'
    New-Item -ItemType Directory -Force -Path $dest | Out-Null
    Copy-Item $dll $dest -Force
    Copy-Item $json $dest -Force

    Write-Host '============================================'
    Write-Host '  安装成功！' -ForegroundColor Green
    Write-Host '============================================'
    Write-Host ''
    Write-Host '请按以下步骤激活模组：'
    Write-Host ''
    Write-Host '  第一步：启动「杀戮尖塔 2」'
    Write-Host '  第二步：在主菜单点击「Mods 模组」按钮'
    Write-Host '  第三步：勾选「Ascension Adjuster」'
    Write-Host '  第四步：重启游戏'
    Write-Host ''
    Write-Host '默认配置：全部角色的天梯等级上限解锁至第 10 级'
    Write-Host '如需单独调整某个角色，请编辑以下文件：'
    Write-Host "  $dest\config.json"
} else {
    Write-Host '============================================'
    Write-Host '  自动安装失败' -ForegroundColor Red
    Write-Host '============================================'
    Write-Host ''
    Write-Host '未能自动找到「杀戮尖塔 2」的安装目录。'
    Write-Host '请手动将以下两个文件复制到指定路径：'
    Write-Host ''
    Write-Host '  需复制的文件：'
    Write-Host '    AscensionAdjuster.dll'
    Write-Host '    AscensionAdjuster.json'
    Write-Host ''
    Write-Host '  目标路径（示例）：'
    Write-Host '    D:\Steam\steamapps\common\Slay the Spire 2\mods\AscensionAdjuster\'
    Write-Host ''
    Write-Host '  若不确定游戏位置，请在 Steam 库中右键游戏'
    Write-Host '  选择「管理」->「浏览本地文件」即可找到。'
}

Write-Host ''
