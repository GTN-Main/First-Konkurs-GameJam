using UnityEngine;
using System.Threading.Tasks;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    async static Task OnStart()
    {
        DebugUtility.WriteInColor($"Bootstrapping...", Color.green);
        await Create();
        await LoadData();
        await Init();
    }

    async static Task Create()
    {
        UnityEngine.Object.DontDestroyOnLoad(
            UnityEngine.Object.Instantiate(
                Resources.Load<GameObject>("Prefabs/Setup/ManagersAndControllers")
            )
        );
        await Task.Yield();
    }

    async static Task LoadData()
    {
        await SavesManager.Load();
    }

    async static Task Init()
    {
        TryInit(GameManager.Instance);
    }
    
    static void TryInit(MonoBehaviour script)
    {
        if (script == null)
        {
            Debug.LogError($"Script {script.name} cannot be initialized. Null reference.");
            return;
        }

        if (script is IInitializable initializable)
        {
            initializable.Init();
        }
    }
}
