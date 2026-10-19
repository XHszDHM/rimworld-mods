# XHszDHM 的 RimWorld 模组

个人 RimWorld 模组仓库。主要是一些汉化包和兼容性补丁。

## 模组列表

| 模组 | 包名 | 支持版本 | 说明 |
|---|---|---|---|
| [更多奥德赛运输机 汉化](mods/CH_更多奥德赛运输机) | `XHszDHM.Odyssey.Transport.Shuttle.Chinese` | 1.6 / 1.7 | More Odyssey Transport Ships 的简体中文翻译 |
| [更好的重力飞船 汉化](mods/CH_更好的重力飞船) | `XHszDHM.Bigger.Gravship.Chinese` | 1.6 / 1.7 | Better Gravship 的简体中文翻译 |
| [VCEPU Waste Fix](mods/VCEPUWasteFix) | `XHszDHM.VCEPUWasteFix` | 1.7 | 修复 VCEPU 的 `Collection was modified` 异常 |

### 详细介绍

**CH_更多奥德赛运输机** — [创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3526744063)

为 *More Odyssey Transport Ships* 添加简体中文翻译。

**CH_更好的重力飞船** — [创意工坊](https://steamcommunity.com/sharedfiles/filedetails/?id=3527516625)

为 *Better Gravship* 添加简体中文翻译。

**VCEPU Waste Fix** — 无工坊版本

*Vanilla Chemfuel Expanded - Unofficial Expansion* 的 `CompWasteProducer_ProduceWaste_Patch.Postfix` 在遍历 `ThingOwner` 的同时调用 `TryDrop`，会抛出 `InvalidOperationException: Collection was modified`，导致机械培育器完成时剩余的废物包卡住。

这个补丁在启动时移除那个有问题的 Postfix，装上一个先快照再遍历的版本。只影响那个出错的补丁，不改游戏内容。

> 原作者已确认这个 bug，但暂时不修。等他修好之后这个补丁就没用了，可以直接删掉。

## 安装

把 `mods/` 下对应的文件夹整个复制到 RimWorld 的 Mods 目录：

```
...\steamapps\common\RimWorld\Mods\
```

然后在游戏内的「模组」界面启用。

## 从源码编译

只有 `VCEPUWasteFix` 需要编译，依赖 **.NET 10 SDK** 和 RimWorld 的引用程序集（`Assembly-CSharp.dll`、`0Harmony.dll`）。

```powershell
cd mods\VCEPUWasteFix
dotnet build -c Release
```

编译产物在 `bin\Release\net10.0\`，把生成的 DLL 复制到 `Assemblies\` 即可。

## 许可

MIT License。详见 [LICENSE](LICENSE)。
