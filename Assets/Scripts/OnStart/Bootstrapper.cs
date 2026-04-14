using System.Threading.Tasks;
using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static async Task OnStart()
    {
        DebugUtility.WriteInColor($"Bootstrapping...", Color.green);
        await Create();
        await LoadData();
        await Init();
    }

    static async Task Create()
    {
        UnityEngine.Object.DontDestroyOnLoad(
            UnityEngine.Object.Instantiate(
                Resources.Load<GameObject>("Prefabs/Setup/ManagersAndControllers")
            )
        );
        await Task.Yield();
    }

    static async Task LoadData()
    {
        await SavesManager.Load();
    }

    static async Task Init()
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
