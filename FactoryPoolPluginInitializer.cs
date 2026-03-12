#if TOOLS
using Godot;
using System;

[Tool]
public partial class FactoryPoolPluginInitializer : EditorPlugin {
	const string AUTOLOAD_NAME = "FactoryPoolManager";
	public override void _EnterTree() {
		string path = ((Resource)GetScript())
					.ResourcePath.GetBaseDir();

		AddAutoloadSingleton(
			AUTOLOAD_NAME,
			path.PathJoin($"{AUTOLOAD_NAME}.tscn")
		);
	}

	public override void _ExitTree() {
		RemoveAutoloadSingleton(AUTOLOAD_NAME);
	}
}
#endif
