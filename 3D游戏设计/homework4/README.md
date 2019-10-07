# 1、基本操作演练
## 1.1 下载 Fantasy Skybox FREE， 构建自己的游戏场景
1）下载 Fantasy Skybox FREE：在Unity的Asset Store中找到Fantasy Skybox FREE的材料包，然后下载并且导入自己的项目中。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007160242458.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
导入成功后多了很多材料：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007160028778.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
2） 构建自己的游戏场景：
在Camera中添加Component，然后添加Skybox，再将相应的Skybox图案添加上去，就能够完成天空盒的创建了，得到自己喜欢的背景了。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007161208825.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
然后创建地形，需要添加Terrain，然后可以在Terrain中创造一些自己的场景，具体方法查看下图：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007161510661.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007162834324.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)
## 1.2 写一个简单的总结，总结游戏对象的使用
游戏对象主要包括：空对象，摄像机，光线，3D物体，声音，UI基于事件的new UI系统和粒子系统与特效，预制的材料
- 空对象（Empty）：不显示却是最常用的对象之一
- 摄像机（Camara）：观察游戏世界的窗口
- 光线（Light）：游戏世界的光源，让游戏世界富有魅力
- 3D物体 ：游戏中的重要组成部分，可以改变其网格和材质，也是很多复杂对象的初始材料
- 声音（Audio）：游戏中的音乐或者声音来源
- 预制材料：方便复杂对象的重复使用
# 2、编程实践：牧师与魔鬼动作分离版
1）本次作业需要在上次作业的基础上，将游戏场景的动作分离出来：
即依然是采用MVC结构实现，与上一次无动作分离版的区别在于，之前对于动作的管理是实现了一个动作类，当鼠标点击船或是人物时，相当于是控制器让船或人物的实例调用动作类来实现运动。这次实践中将船或人物与动作分离开来，单独实现了一个动作管理器，鼠标点击船或是人物时，相当于是控制器发送请求给动作管理器，动作管理器来实现船或人物的运动。
2）主要思路：按照下图将上次的代码进行分解：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20191007165814956.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2h1YW5nc2htMjM=,size_16,color_FFFFFF,t_70)

3）接下来是动作基类的实现（课程中有）：
设计要点：
- ScriptableObject是不需要绑定GameObject对象的可编程基类。这些对象受Unity引擎场景管理
- protected SSAction()是防止用户自己new对象
- 使用virtual申明虚方法，通过重写实现多态。这样继承者就能明确使用Start和Update编程游戏对象行为
- 利用接口实现消息通知，避免与动作管理者直接依赖

```go
public class SSAction : ScriptableObject
{
    public bool enable = true;
    public bool destroy = false;
 
    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }
 
    protected SSAction() {}
 
    public virtual void Start() {
        throw new System.NotImplementedException();
    }
 
    public virtual void Update() {
        throw new System.NotImplementedException();
    }
}
```
4）简单动作实现：
设计目的：游戏中移动的动作，通过传入游戏对象的位置和设置好的动作，就能够使游戏对象移动起来
```go
public class CCMoveToAction : SSAction {
	public Vector3 target;
	public float speed;
 
	public static CCMoveToAction GetSSAction(Vector3 target, float speed) {
		CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
		action.target = target;
		action.speed = speed;
		return action;
	}
 
	public override void Update () {
		this.transform.position = Vector3.MoveTowards(this.transform.position,target,speed);
		if(this.transform.position == target) {
			this.destroy = true;
			this.callback.SSActionEvent(this);
		}
	}
 
	public override void Start() {}
}
```
5）顺序动作组合类实现：
代码重点：
- repeat的值为-1表示动作无限循环，而start则表示动作开始
- Update的重写则是表示执行当前的动作
- SSActionEvent则是一个回调通知的动作，当收到当前动作执行完成后，则推下一个动作，如果完成一次循环，则减少它的次数。如果当所有动作完成，就通知动作的管理者，将其销毁。
- Start的重写则是表示，在执行动作前，为每个动作注入当前动作的游戏对象，并将自己作为动作事件的接收者。

```go
public class CCSequenceAction : SSAction, ISSActionCallback {
    public List<SSAction> sequence;
    public int repeat = -1; //repeat forever
    public int start = 0;
 
    public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence) {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }
 
    public override void Start() {
        foreach (SSAction action in sequence) {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;
            action.Start();
        }
    }
 
    public override void Update() {
        if (sequence.Count == 0) return;
        if (start < sequence.Count)
            sequence[start].Update();
    }
 
    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Completed,
     int intParam = 0, string strParam = null, Object objectParam = null) {
        source.destroy = false;
        this.start++;
        if (this.start >= sequence.Count) {
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0) {
                this.destroy = true;
                this.callback.SSActionEvent(this);
            }
            else {
                sequence[start].Start();
            }
        }
    }
 
    private void OnDestroy() {
        //destory
    }
}
```
6）动作事件接口定义：在定义了时间处理接口以后，所有的事件管理者都必须实现这个接口来实现事件调度。所以，组合事件需要实现它，事件管理器也必须实现它。

```go
public enum SSActionEventType : int { Started, Completed }
 
public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Completed,
        int intParam = 0, string strParam = null, Object objectParam = null);
}
```

7）动作管理基类 – SSActionManager：实现了所有动作的基本管理

```go
public class SSActionManager : MonoBehaviour, ISSActionCallback {                     //action管理器

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    //将执行的动作的字典集合,int为key，SSAction为value
    private List<SSAction> waitingAdd = new List<SSAction>();                       //等待去执行的动作列表
    private List<int> waitingDelete = new List<int>();                              //等待删除的动作的key                

    protected void Update(){
        foreach (SSAction ac in waitingAdd){
            actions[ac.GetInstanceID()] = ac;                                      //获取动作实例的ID作为key
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions){
            SSAction ac = kv.Value;
            if (ac.destroy) waitingDelete.Add(ac.GetInstanceID());
            else if (ac.enable) ac.Update();
        }

        foreach (int key in waitingDelete){
            SSAction ac = actions[key];
            actions.Remove(key);
            Object.Destroy(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager){
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null){
        //牧师与魔鬼的游戏对象移动完成后就没有下一个要做的动作了，所以回调函数为空
    }
}

```
8）裁判类的实现：对当前局面胜负关系的判断

```go
public class Judger : System.Object {
    private static Judger _instance; 
    public static Judger getInstance() { //使用单例模式
        if (_instance == null) _instance = new Judger ();
        return _instance;
    }
    public int check(CoastCon fromCoast,CoastCon toCoast,BoatCon boat) {	// 0->not finish, 1->lose, 2->win
        int fromP = 0, fromD = 0, toP = 0, toD = 0;
        int[] fromCount = fromCoast.getCharacterNum();
        fromP += fromCount[0];
        fromD += fromCount[1];

        int[] toCount = toCoast.getCharacterNum ();
        toP += toCount[0];
        toD += toCount[1];

        if (toP + toD == 6) return 2; //win
        int[] boatCount = boat.getCharacterNum ();
        if (boat.getStatus () == -1) {
            toP += boatCount[0]; toD += boatCount[1];	// boat at toCoast
        }
        else {
            fromP += boatCount[0]; fromD += boatCount[1];	// boat at fromCoast	
        }
        if (fromP < fromD && fromP > 0) return 1; //lose		
        if (toP < toD && toP > 0) return 1; //lose
        return 0;			// not finish
    }
}

```
9）[视频链接](https://v.youku.com/v_show/id_XNDM4OTE3OTE0NA==.html?spm=a2h3j.8428770.3416059.1)
