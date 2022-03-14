using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cheat.Helpers
{
	public class DumbHook
	{
		public MethodInfo OriginalMethod { get; private set; }

		public MethodInfo HookMethod { get; private set; }

		public DumbHook()
		{
			this.original = null;
			this.OriginalMethod = (this.HookMethod = null);
		}

		public DumbHook(MethodInfo orig, MethodInfo hook)
		{
			this.original = null;
			this.Init(orig, hook);
		}

		public MethodInfo GetMethodByName(Type typeOrig, string nameOrig)
		{
			return typeOrig.GetMethod(nameOrig);
		}

		public DumbHook(Type typeOrig, string nameOrig, Type typeHook, string nameHook)
		{
			this.original = null;
			this.Init(this.GetMethodByName(typeOrig, nameOrig), this.GetMethodByName(typeHook, nameHook));
		}

		public void Init(MethodInfo orig, MethodInfo hook)
		{
			bool flag = orig == null || hook == null;
			if (flag)
			{
				throw new ArgumentException("Both original and hook need to be valid methods");
			}
			RuntimeHelpers.PrepareMethod(orig.MethodHandle);
			RuntimeHelpers.PrepareMethod(hook.MethodHandle);
			this.OriginalMethod = orig;
			this.HookMethod = hook;
		}

		public unsafe void Hook()
		{
			bool flag = null == this.OriginalMethod || null == this.HookMethod;
			if (flag)
			{
				throw new ArgumentException("Hook has to be properly Init'd before use");
			}
			bool flag2 = this.original != null;
			if (!flag2)
			{
				IntPtr functionPointer = this.OriginalMethod.MethodHandle.GetFunctionPointer();
				IntPtr functionPointer2 = this.HookMethod.MethodHandle.GetFunctionPointer();
				bool flag3 = IntPtr.Size == 8;
				if (flag3)
				{
					this.original = new byte[12];
					uint newProtect;
					DumbHook.Import.VirtualProtect(functionPointer, 12U, 64U, out newProtect);
					byte* ptr = (byte*)((void*)functionPointer);
					int num = 0;
					while ((long)num < 12L)
					{
						this.original[num] = ptr[num];
						num++;
					}
					*ptr = 72;
					ptr[1] = 184;
					*(IntPtr*)(ptr + 2) = functionPointer2;
					ptr[10] = byte.MaxValue;
					ptr[11] = 224;
					DumbHook.Import.VirtualProtect(functionPointer, 12U, newProtect, out newProtect);
				}
				else
				{
					this.original = new byte[7];
					uint newProtect;
					DumbHook.Import.VirtualProtect(functionPointer, 7U, 64U, out newProtect);
					byte* ptr2 = (byte*)((void*)functionPointer);
					int num2 = 0;
					while ((long)num2 < 7L)
					{
						this.original[num2] = ptr2[num2];
						num2++;
					}
					*ptr2 = 184;
					*(IntPtr*)(ptr2 + 1) = functionPointer2;
					ptr2[5] = byte.MaxValue;
					ptr2[6] = 224;
					DumbHook.Import.VirtualProtect(functionPointer, 7U, newProtect, out newProtect);
				}
			}
		}

		public unsafe void Unhook()
		{
			bool flag = this.original == null;
			if (!flag)
			{
				uint num = (uint)this.original.Length;
				IntPtr functionPointer = this.OriginalMethod.MethodHandle.GetFunctionPointer();
				uint num2;
				DumbHook.Import.VirtualProtect(functionPointer, num, 64U, out num2);
				byte* ptr = (byte*)((void*)functionPointer);
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num))
				{
					ptr[num3] = this.original[num3];
					num3++;
				}
				DumbHook.Import.VirtualProtect(functionPointer, num, 64U, out num2);
				this.original = null;
			}
		}

		private const uint HOOK_SIZE_X64 = 12U;

		private const uint HOOK_SIZE_X86 = 7U;

		private byte[] original;

		internal class Import
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool VirtualProtect(IntPtr address, uint size, uint newProtect, out uint oldProtect);
		}
	}
}
