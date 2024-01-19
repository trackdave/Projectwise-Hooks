using DllExporterNet4;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Projectwise_Hooks
{
	public static class HookSampleManagedClass
	{
		const int IDOK = 1;

		public static HookCallback Hooks = new HookCallback();

		public static PWWrapper.GenericHookFunction CreateFolderPreHook;
		public static PWWrapper.GenericHookFunction ModifyFolderPreHook;

		[DllExport]
		public static int CustomInitialize(int ulMask, IntPtr lpReserved)
		{
			CreateFolderPreHook = Hooks.LoginPreHook;
			PWWrapper.aaApi_AddHook((int)PWWrapper.HookIdentifiers.AAHOOK_CREATE_PROJECT, PWWrapper.HookTypes.AAPREHOOK, CreateFolderPreHook);
			return IDOK;
		}
	}

	public class HookCallback
	{
		public unsafe int LoginPreHook
		(
			int hookId,
			int hookType,
			[In, Out] IntPtr aParam1,
			int aParam2,
			ref int pResult
		)
		{
			//_AAPROCREATE_PARAM pProjectParam = (_AAPROCREATE_PARAM)Marshal.PtrToStructure(aParam1, typeof(_AAPROCREATE_PARAM));
			//string name = pProjectParam.lptstrName;
			//if (name.Length > 20)
			//{
			//	MessageBox.Show("Folder name can not have more than 20 characters.", "Error Creating Folder");
			//	return (int)PWWrapper.HookActions.AAHOOK_ERROR;
			//}

			return (int)PWWrapper.HookActions.AAHOOK_SUCCESS;
		}

		/// <summary>
		/// The AAPOSTHOOK hook type specifies that the hook handler function is being registered to be called
		/// after successful processing of the event action.
		/// All post-hooks should return AAHOOK_SUCCESS.
		/// Currently, the return values of post-hooks are ignored, and the return value from the hook handler
		/// function cannot be used to prevent the execution of any other post-hook handlers in the chain.
		/// To register a handler to be called regardless of the success or failure of the event action,
		/// register a Post-Fail Hook.
		/// </summary>
		/// <param name="hookId">The id of the hook that is being called.</param>
		/// <param name="hookType">Type type of hook -Pre, Action, or Post.</param>
		/// <param name="aParam1">Varies by hook Id. ALWAYS review the documentation!</param>
		/// <param name="aParam2">Varies by hook Id. ALWAYS review the documentation!</param>
		/// <param name="pResult">Varies by hook Id. ALWAYS review the documentation!</param>
		/// <returns>Varies by hook type.</returns>
		public unsafe int LoginPostHook
		(
			int hookId,
			int hookType,
			[In, Out] IntPtr aParam1,
			int aParam2,
			ref int pResult
		)
		{
			// "best practice" is it check to see if the value of aParam2 is one of the types you are interested in
			// otherwise just let the hook continue.
			// aParam2 for AAHOOK_LOGIN can be one of three values.
			// The only one in our case that we care about is AAOPER_USER_LOGIN.
			//if (aParam2 != (int)HookParamValue.AAOPER_USER_LOGIN)
			//{
			//    return (int)PWWrapper.HookActions.AAHOOK_SUCCESS;  // let the hook chain continue
			//}

			// "best practice" is to cast aParam1 for the structure/type for this particular hook.
			// Note that the cast type can depend upon the VALUE of aParam2!
			// In this case, since we only care about AAOPER_USER_LOGIN,
			// we will cast it to an AALOGIN_PARAM structure.
			// For this posthook, we will only use the username...
			//_AALOGIN_PARAM* pLoginParam = (_AALOGIN_PARAM*)aParam1;

			// There may be other ways to get to the strings in the structure
			// but this way works...
			//string userName = Marshal.PtrToStringAuto((IntPtr)pLoginParam->lpctstrUser);
			//string userName = "dc";
			//StringBuilder sbDSN = new StringBuilder(1024);
			//PWWrapper.aaApi_GetActiveDatasourceName(sbDSN, sbDSN.Capacity);
			//DialogResult result = MessageBox.Show($"Logged in to '{sbDSN}'' as '{userName}'",
			//    "Post Login Message",
			//    MessageBoxButtons.OK);
			//    MessageBoxButtons.OK);

			//_AAPROCREATE_PARAM* pProjectParam = (_AAPROCREATE_PARAM*)aParam1;
			_AAPROCREATE_PARAM pProjectParam = (_AAPROCREATE_PARAM)Marshal.PtrToStructure(aParam1, typeof(_AAPROCREATE_PARAM));
			//int folderId = pProjectParam->lProjectId;
			//string folderName = pProjectParam->lptstrName;

			MessageBox.Show($"About to create folder.\n{pProjectParam.lProjectId} -> {pProjectParam.lptstrName} {aParam2} {pResult}");
			//MessageBox.Show($"Folder created.\n{folderId} -> {folderName} {aParam2} {pResult}");
			return (int)PWWrapper.HookActions.AAHOOK_SUCCESS;
		}
	}
}
