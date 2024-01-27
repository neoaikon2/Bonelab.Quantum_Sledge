using MelonLoader;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace QuantumSledge
{
    public class ModLoader : MelonMod
    {
		public override void OnInitializeMelon()
		{
#if DEBUG
			ClassInjector.RegisterTypeInIl2Cpp<QuantumSledgeMan>(true);
#else
			ClassInjector.RegisterTypeInIl2Cpp<QuantumSledgeMan>();
#endif
			base.OnInitializeMelon();
		}
	}
}
