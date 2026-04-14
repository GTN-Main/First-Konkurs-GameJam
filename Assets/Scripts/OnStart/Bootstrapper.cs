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
        await TryInit(GameManager.Instance);
        await TryInit(PlayerSpawnManager.Instance);
        await TryInit(HealthManager.Instance);
        await TryInit(EnemiesSpawnManager.Instance);
        await TryInit(BoxesSpawnManager.Instance);
        await TryInit(WaterContainerManager.Instance);
        await TryInit(MyParticleSystem.Instance);
    }
    
    static async Task TryInit(MonoBehaviour script)
    {
        if (script == null)
        {
            Debug.LogError($"Script {script.name} cannot be initialized. Null reference.");
            return;
        }

        if (script is IInitializable initializable)
        {
            await initializable.Init();
        }
    }
}
