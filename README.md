# ProjectBaseUnity
将游戏的功能整理分类，集合了常用的单例，降低样板代码，适合mini、小型游戏开发、GameJam，既可作为扩展，也能作为基础框架。 包含 资源管理、UI 管理、事件、持久化、对象池、音乐、输入、常量管理、Mono等常见功能。

# ProjectBase 小框架介绍与使用指南

本指南面向首次接触 `ProjectBase` 的开发者，结合示例与图示，帮助快速理解和上手。

---

## 1. 目录结构

```
Assets/Scripts/ProjectBase/
├─ base/                 # 基础单例
│  ├─ SingletonBase.cs
│  └─ SingletonMono.cs
├─ UI/                   # UI 管理与面板基类
│  ├─ UIManager.cs
│  └─ BasePanel.cs
├─ Events/               # 事件总线
│  └─ EventsManager.cs
├─ Input/                # 输入统一入口
│  └─ InputManager.cs
├─ Mono/                 # Mono 中枢，提供 Update/协程入口
│  └─ MonoController.cs
├─ ResManager/           # 资源加载（同步/异步/StreamingAssets）
│  └─ ResManager.cs
├─ ObjectPool/           # 对象池
│  ├─ ObjectPool.cs
│  └─ PoolData.cs
├─ Music/                # BGM/音效管理
│  └─ MusicManager.cs
├─ DataManager/          # 数据存储（PlayerPrefs/Json）
│  ├─ PPDataManager.cs
│  ├─ JsonManager.cs
│  └─ Serialization.cs
└─ Constants/            # 常量/事件名集中管理
   └─ Constants.cs
```

示例与入口：
- `Assets/Scripts/PBFramworkSample/`：`PBFPanel.cs`、`PBWinPanelTest1.cs`、`PBWinPanelTest2.cs`
- `Assets/Scripts/Game/`：`PBFSampleGameStart.cs`

---

## 2. 架构总览

核心思想：通过统一的单例管理与事件总线，实现 UI、输入、资源、动画、音频、对象池、数据存储的模块化、低耦合协作。

ASCII 架构图（简化）：
```
+--------------------+        +------------------+
|  MonoController    | <----> |  Update Loop     |
| (SingletonMono)    |        +------------------+
+---------+----------+
          |
          v
+---------+----------+     +------------------+     +------------------+
|  UIManager         | <--> |  EventsManager  | <--> | InputManager     |
| (panel层/动画队列) |     | (事件总线)       |     | (输入事件触发)   |
+---------+----------+     +------------------+     +------------------+
          |
          v
+-------------------+     +------------------+     +------------------+
|  ResManager       |     |  ObjectPool      |     | MusicManager     |
| (资源加载)        |     | (缓存复用)       |     | (BGM/音效)       |
+-------------------+     +------------------+     +------------------+
          |
          v
+-------------------+
| DataManager       |
| (PlayerPrefs/Json)|
+-------------------+
```

UI 层级结构（Canvas 子节点）：
```
MainCanvas (tag: MainCanvas)
├─ bot    # sort order = 0
├─ mid    # sort order = 20
├─ top    # sort order = 40
└─ system # sort order = 60
```

---

## 3. 核心模块与职责

- 单例基类
  - `SingletonBase<T>`：非 `MonoBehaviour` 单例（`base/SingletonBase.cs:5`）
  - `SingletonMono<T>`：`MonoBehaviour` 单例（自动创建、`DontDestroyOnLoad`）（`base/SingletonMono.cs:5, 28`）
- UI 管理
  - `UIManager`：查找/自动加载 `MainCanvas` 与 `EventSystem`、面板层级管理、动画队列、屏幕适配（`UI/UIManager.cs:80, 83-106, 167-235, 325-350`）
  - `BasePanel`：面板基类，控件收集与按钮/Toggle回调、动画添加（`UI/BasePanel.cs:10, 54-64, 111-118, 80-105`）
- 事件总线
  - `EventsManager`：类型安全事件订阅/触发/移除（`Events/EventsManager.cs:45, 70, 96`）
- 输入管理
  - `InputManager`：统一开关与输入事件触发（鼠标左右键、Escape）（`Input/InputManager.cs:13-40`）
- Mono 中枢
  - `MonoController`：统一 `Update` 入口与协程平台（`Mono/MonoController.cs:7-33`）
- 资源加载
  - `ResManager`：`Resources` 同步/异步加载、`StreamingAssets` 文本读取（`ResManager/ResManager.cs:17, 32, 51`）
- 对象池
  - `ObjectPool` / `PoolData`：按名称组织的对象复用（`ObjectPool/ObjectPool.cs:6, 20-46, 52-66`）
- 音频管理
  - `MusicManager`：BGM/音效播放、音量与回收（`Music/MusicManager.cs:36-51, 75-96, 101-118, 121-128`）
- 数据存储（推荐使用 Easy Save 3 插件代替PlaerPrefs重新实现，ES3通常情况下，无需考虑序列化问题）
  - `PPDataManager`：通过反射将复杂对象持久化到 `PlayerPrefs`（支持 `IList`/`IDictionary`）（`DataManager/PPDataManager.cs:19-33, 35-98, 103-126, 129-214`）
  - `JsonManager`：序列化到 `persistentDataPath` 的 `.json` 文件（`DataManager/JsonManager.cs:15-29, 33-46, 48-55`）
  - `Serialization.cs`：扩展 `JsonUtility` 支持 `List<T>`/`Dictionary<TKey, TValue>`（`DataManager/Serialization.cs:11-21, 25-57`）
- 常量集中管理
  - `Constants.cs`：事件名/枚举等统一定义（`Constants/Constants.cs:17-22`）

---

## 4. 优点与设计取舍

- 轻量模块化：各模块职责单一，互相低耦合，易于替换/扩展
- 全局统一入口：事件总线、输入抽象、资源加载统一化，减少散落逻辑
- UI 层级规范：`bot/mid/top/system` 清晰分层，支持返回键关闭栈顶面板（`UIManager.cs:115-133`）
- BasePanel：面板基类，控件收集与按钮/Toggle回调并且自动绑定、动画添加（`UI/BasePanel.cs:10, 54-64, 111-118, 80-105`）
- 动画队列：面板动画顺序播放、可跳过（`UIManager.cs:323-350`）
- 对象池与资源复用：减少实例化与 GC 压力（`ObjectPool.cs:20-46, 52-66`）
- 数据持久化：`PlayerPrefs` 与 `Json` 双方案，支持复杂结构
- 示例齐全：`PBFSampleGameStart` 与 `PBFramworkSample` 提供开箱即用演示

---

## 5. 快速上手（5 步）

1) 准备 `MainCanvas` 与 `EventSystem`
   - 场景中放置并设置 Tag：`MainCanvas`、`MainEventSystem`
   - 或提供 `Resources/UI/MainCanvas.prefab` 与 `Resources/UI/EventSystem.prefab`（自动加载）
2) 面板预制体
   - 资源默认放在 `Resources/UI/`（可在 `UIManager.cs:122` 自定义），命名为 `YourPanelName`
   - 绑定脚本继承 `BasePanel`，建议添加 `CanvasGroup` 赋值到 `cG` 字段（用于交互锁）
3) 游戏入口
   - 在任意 `MonoBehaviour` 或 `SingletonMono` 中调用 `UIManager.Instance.ShowPanel<T>("PanelName")`
4) 抽象输入与事件（可选）
   - `InputManager.Instance.SwitchInputCheck(true)` 开启输入检查
   - 使用 `EventsManager` 实现模块通信
5) 资源与音频
   - 用 `ResManager` 统一加载，`MusicManager` 播放 BGM/音效

---

## 6. 常用范式与示例

- 显示/隐藏面板
  ```csharp
  // 显示（默认加载 Resources/UI/YourPanel）
  UIManager.Instance.ShowPanel<YourPanel>("YourPanel"); // UI/UIManager.cs:174

  // 指定层级和自定义资源路径前缀
  UIManager.Instance.ShowPanel<YourPanel>("YourPanel", UIManager.UIM_layer.Top, null, "CustomUIPrefix/");

  // 隐藏/销毁
  UIManager.Instance.HidePanel("YourPanel"); // UI/UIManager.cs:282-292

  // 获取面板（可配合 WaitUntil 写瀑布流）
  var pnl = UIManager.Instance.GetPanel<YourPanel>("YourPanel"); // UI/UIManager.cs:315-321
  ```

- 在面板中收集控件与按钮回调
  ```csharp
  public class YourPanel : BasePanel {
      protected override void OnBtnClick(string btnName) {
          if (btnName == "CloseBtn") UIManager.Instance.HidePanel("YourPanel");
      }
      void ToggleExample() {
          var toggle = GetControler<Toggle>("YourToggle");
      }
  } // UI/BasePanel.cs:111-118, 132-145, 152-179
  ```

- 事件总线（类型安全）
  ```csharp
  // 订阅
  EventsManager.Instance.AddEventsListener<int>("EventTest", OnEvent);
  // 触发
  EventsManager.Instance.EventTrigger<int>("EventTest", 42);
  // 移除
  EventsManager.Instance.RemoveListener<int>("EventTest", OnEvent);
  void OnEvent(int i) { Debug.Log(i); }
  // Events/EventsManager.cs:45, 70, 96
  ```

- 输入统一入口
  ```csharp
  InputManager.Instance.SwitchInputCheck(true); // 开启输入
  // 监听 ESC 在 UIManager 构造函数中已处理（关闭栈顶面板）
  // Input/InputManager.cs:13-40
  ```

- UI 动画队列（需要 DOTween）
  ```csharp
  protected override void ShowMe() {
      var seq = DOTween.Sequence();
      seq.Append(cG.DOFade(1, 0.2f));
      AddUiAnimation(seq, interactable:false); // BasePanel.cs:80-105
  }
  ```

- 资源加载
  ```csharp
  var go = ResManager.Instance.Load<GameObject>("UI/YourPanel");                // 同步
  ResManager.Instance.LoadAsync<AudioClip>("sounds/click", clip => { /*...*/ }); // 异步
  var text = ResManager.Instance.LoadFromStreamingAssets(path);                  // 文本读取
  // ResManager/ResManager.cs:17, 32, 51
  ```

- 对象池
  ```csharp
  ObjectPool.Instance.GetObj("Prefabs/Bullet", obj => {
      // 使用 obj
  }, parent: someTransform, ifAsync: true);
  ObjectPool.Instance.TStoreObj("Bullet", obj); // 归还
  // ObjectPool/ObjectPool.cs:20-46, 52-66
  ```

- 音频
  ```csharp
  MusicManager.Instance.PlayBGM("music/bgm_main");
  MusicManager.Instance.PlaySound("sounds/click");
  MusicManager.Instance.ChangeSoundVolume(0.5f);
  MusicManager.Instance.StopBGM();
  // Music/MusicManager.cs:36-72, 75-96
  ```

- 数据存储（复杂对象 → PlayerPrefs）
  ```csharp
  PPDataManager.Instance.SaveData(playerData, "Player");           // 写入
  var obj = PPDataManager.Instance.LoadData(typeof(PlayerData), "Player"); // 读取
  // DataManager/PPDataManager.cs:19-33, 103-126
  ```

- JSON 存储
  ```csharp
  JsonManager.Instance.SaveJson(playerData, "player");
  var loaded = JsonManager.Instance.LoadJson<PlayerData>("player");
  JsonManager.Instance.DeletJson("player");
  // DataManager/JsonManager.cs:15-29, 33-46, 48-55
  ```

- 屏幕适配辅助
  ```csharp
  var ratio = UIManager.AspectRatio;    // UI/UIManager.cs:46-49
  var wScale = UIManager.GetWidthScale; // UI/UIManager.cs:53-60
  var hScale = UIManager.GetHeightScale;// UI/UIManager.cs:62-69
  ```

- 示例入口（瀑布流展示）
  ```csharp
  // Game/PBFSampleGameStart.cs:15, 21-35
  UIManager.Instance.ShowPanel<PBFPanel>("PBFPanel", UIManager.UIM_layer.Bottom, null, "ProjecBaseSampleUI/");
  StartCoroutine(UIFalls()); // 依次展示 PBWinPanelTest1 -> PBWinPanelTest2 -> PBFPanel
  ```

---

## 7. 使用建议与注意事项

- 保证存在 `Camera.main`；`UIManager` 会将 Canvas 设为 `ScreenSpaceCamera` 并绑定主摄像机（`UI/UIManager.cs:103-106`）
- 若不在场景中手动放置 `MainCanvas`/`EventSystem`，需在 `Resources/UI/` 下提供同名预制体（`UI/UIManager.cs:83-91`）
- 面板根节点建议为 `RectTransform`，以便 `anchorMin/Max` 与 `sizeDelta` 正常设置（`UI/UIManager.cs:211-220`）
- 复杂面板动画使用 `AddUiAnimation` 管理交互与队列，避免并发冲突（`BasePanel.cs:80-105`）
- `withoutEscBtn = true` 可使面板不被返回键逻辑管理（`UI/UIManager.cs:135-139`）
- 使用对象池时，归还前确保对象状态复位（位置/缩放/激活等）；框架已在取出时重置 `localScale` 为 `Vector3.one`（`PoolData.cs:33`）
- 数据持久化命名规则：`keyName_Type_FieldType_FieldName`，更改字段名会导致读取失败（`PPDataManager.cs:29-33, 116-122`）
- 事件总线类型需一致；同名事件若类型不匹配会替换为新类型（`EventsManager.cs:45-64`）

---

## 8. 示例文件导航（关联代码位置）

- `PBFSampleGameStart`（入口展示与协程瀑布流）：`Assets/Scripts/Game/PBFSampleGameStart.cs:15, 21-35`
- `PBFPanel`（示例面板，演示事件订阅与按钮处理）：`Assets/Scripts/PBFramworkSample/PBFPanel.cs:12-19, 26-42, 47-71`
- `PBWinPanelTest1/2`（简单关闭按钮示例）：`Assets/Scripts/PBFramworkSample/PBWinPanelTest1.cs:7-16`、`PBWinPanelTest2.cs:7-16`

---

## 9. 扩展建议

- 在 `Constants` 中集中维护事件名与关键常量（`Constants/Constants.cs:17-22`）
- 按模块扩展：例如新增 `SceneManager`/`NetworkManager`，均按 `SingletonBase<T>` 或 `SingletonMono<T>` 组织
- 引入更丰富的 UI 动画库时，保持由 `BasePanel` 统一入队管理，避免并发

---

## 10. FAQ

- Q：不放 `MainCanvas`/`EventSystem` 在场景里可以吗？
  - A：可以，`UIManager` 会自动从 `Resources/UI/` 加载同名预制体（`UI/UIManager.cs:83-91`）
- Q：如何拦截返回键不关闭当前面板？
  - A：将 `BasePanel.withoutEscBtn = true`（`UI/UIManager.cs:135-139`）
- Q：如何一次加载多个面板并顺序播放动画？
  - A：每个面板使用 `AddUiAnimation` 入队，`UIManager` 通过队列顺序播放（`UI/UIManager.cs:323-350`）

旧版文档连接🔗：
https://lcndm0b2t06l.feishu.cn/wiki/DyR2wSuZoiBDjEkvjrTcOnd9nMg?from=from_copylink
