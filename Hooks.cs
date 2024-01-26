using DllExporterNet4;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Projectwise_Hooks.ParamStructures;

namespace Projectwise_Hooks
{
	public static class HookSampleManagedClass
	{
		const int IDOK = 1;

		public static HookCallback Hooks = new HookCallback();

		public static PWAPI.GenericHookFunction CreateModifyFolderHook;
		public static PWAPI.GenericHookFunction CreateModifyDocumentHook;

		[DllExport]
		public static int CustomInitialize(int ulMask, IntPtr lpReserved)
		{
			CreateModifyFolderHook = Hooks.CreateModifyFolderHook;
			CreateModifyDocumentHook = Hooks.CreateModifyDocumentHook;
			PWAPI.aaApi_AddHook((int)PWAPI.HookIdentifiers.AAHOOK_CREATE_PROJECT, (int)PWAPI.HookTypes.AAPREHOOK, CreateModifyFolderHook);
			PWAPI.aaApi_AddHook((int)PWAPI.HookIdentifiers.AAHOOK_MODIFY_PROJECT, (int)PWAPI.HookTypes.AAPREHOOK, CreateModifyFolderHook);
			PWAPI.aaApi_AddHook((int)PWAPI.HookIdentifiers.AAHOOK_CREATE_DOCUMENT, (int)PWAPI.HookTypes.AAPREHOOK, CreateModifyDocumentHook);
			PWAPI.aaApi_AddHook((int)PWAPI.HookIdentifiers.AAHOOK_MODIFY_DOCUMENT, (int)PWAPI.HookTypes.AAPREHOOK, CreateModifyDocumentHook);
			return IDOK;
		}
	}

	public class HookCallback
	{
		public unsafe int CreateModifyFolderHook(int hookId, int hookType, [In, Out] IntPtr aParam1, int aParam2,ref int pResult)
		{
			AAPROCREATE_PARAM pProjectParam = (AAPROCREATE_PARAM)Marshal.PtrToStructure(aParam1, typeof(AAPROCREATE_PARAM));
			string name = pProjectParam.lptstrName;
			if (name.Length > 25)
			{
				MessageBox.Show("Folder name can not have more than 25 characters.", "Error Creating/Modifying Folder");
				return (int)PWAPI.HookActions.AAHOOK_ERROR;
			}
			return (int)PWAPI.HookActions.AAHOOK_SUCCESS;
		}

		public unsafe int CreateModifyDocumentHook(int hookId, int hookType, [In, Out] IntPtr aParam1, int aParam2, ref int pResult)
		{
			AaDocParam pDocParam = (AaDocParam)Marshal.PtrToStructure(aParam1, typeof(AaDocParam));
			var name = string.IsNullOrEmpty(pDocParam.lpctstrFileName) ? pDocParam.lpctstrName : pDocParam.lpctstrFileName;
			var path = PWAPI.GetProjectNamePath(pDocParam.lProjectId);
			if (!string.IsNullOrEmpty(path))
			{
				var limit = 100;
				var fullPath = $"{path}\\{name}";
				if (fullPath.Length > limit)
				{
					var extra = fullPath.Length - limit;
					MessageBox.Show($"The document {fullPath} is {extra} characters over the limit. Please shorten the document name.", "Error Creating/Modifying Document");
					return (int)PWAPI.HookActions.AAHOOK_ERROR;
				}
			}
			return (int)PWAPI.HookActions.AAHOOK_SUCCESS;
		}
	}
}
