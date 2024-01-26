using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static Projectwise_Hooks.ParamStructures;

namespace Projectwise_Hooks
{
	public static class PWAPI
	{
		static PWAPI()
		{
			try
			{
				aaApi_Initialize(512);
			}
			catch (DllNotFoundException dlfne)
			{
				string sMessage = dlfne.Message;
				AppendProjectWiseDllPathToEnvironmentPath();
			}
			catch (BadImageFormatException badImage)
			{
				string sMessage = badImage.Message;
				AppendProjectWiseDllPathToEnvironmentPath();
			}
			finally
			{
				aaApi_Initialize(512);
			}
		}

		#region System Functions
		public static bool Is64Bit()
		{
			return IntPtr.Size == 8;
		}

		public static string GetProjectWisePath()
		{
			string installDirectory = null;

			try
			{
				if (Is64Bit())
				{
					Version minVersion = new Version("08.11");
					string[] regKeys = new string[]{
					"SOFTWARE\\Bentley\\ProjectWise Explorer",
					"SOFTWARE\\Bentley\\ProjectWise"
				};

					foreach (string regKeyPath in regKeys)
					{
						RegistryKey regLocalMachine = Registry.LocalMachine;
						RegistryKey regKey = regLocalMachine.OpenSubKey(regKeyPath);

						if (regKey != null)
						{
							string[] versions = regKey.GetSubKeyNames();
							if (versions != null)
							{
								for (int i = 0; i < versions.Length; i++)
								{
									RegistryKey versionSubKey = regKey.OpenSubKey(versions[i]);

									if (versionSubKey == null)
										continue;

									Version version = new Version(versions[i]);

									if (version >= minVersion)
									{
										installDirectory = (string)versionSubKey.GetValue("PathName");

										if (string.IsNullOrEmpty(installDirectory))
										{
											installDirectory = (string)versionSubKey.GetValue("Path");
										}

										minVersion = version;
									}
								}
								if (!string.IsNullOrEmpty(installDirectory))
									break;
							}
						}
					}
					if (string.IsNullOrEmpty(installDirectory))
						throw new ApplicationException("Registry search could not find installation directory for a ProjectWise version matching minimum required version '" +
							minVersion + "'.\nMake sure a ProjectWise version matching the above version is installed on this system.");
				}
				else
				{

					Version minVersion = new Version("08.01");
					string[] regKeys = new string[]{"SOFTWARE\\Wow6432Node\\Bentley\\ProjectWise Explorer",
													"SOFTWARE\\Wow6432Node\\Bentley\\ProjectWise Administrator",
													"SOFTWARE\\Bentley\\ProjectWise Explorer",
												   "SOFTWARE\\Bentley\\ProjectWise Administrator"};

					foreach (string regKeyPath in regKeys)
					{
						RegistryKey regLocalMachine = Microsoft.Win32.Registry.LocalMachine;
						RegistryKey regKey = regLocalMachine.OpenSubKey(regKeyPath);

						if (regKey != null)
						{
							string[] versions = regKey.GetSubKeyNames();
							if (versions != null)
							{
								for (int i = 0; i < versions.Length; i++)
								{
									RegistryKey versionSubKey = regKey.OpenSubKey(versions[i]);
									if (versionSubKey == null)
										continue;

									string sVersion = (string)versionSubKey.GetValue("Version");
									if (sVersion == null)
										continue;
									Version version = new Version(sVersion);

									if (version >= minVersion)
									{
										installDirectory = (string)versionSubKey.GetValue("PathName");
										minVersion = version;
									}
								}
								if (installDirectory != null)
									break;
							}
						}
					}
					if (installDirectory == null)
						throw new ApplicationException("Registry search could not find installation directory for a ProjectWise version matching minimum required version '" +
							minVersion + "'.\nMake sure a ProjectWise version matching the above version is installed on this system.");
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}

			return installDirectory;
		}

		private static void AppendProjectWiseDllPathToEnvironmentPath()
		{
			try
			{
				string installDirectory = GetProjectWisePath();
				if (installDirectory != null)
				{
					installDirectory += "\\bin";
					AppendToEnvironmentPath(installDirectory);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}
		private static void AppendToEnvironmentPath(string path)
		{
			int maxLength = 32767;

			if (ConfigurationManager.AppSettings["Path"] != null)
			{
				Environment.SetEnvironmentVariable("Path", ConfigurationManager.AppSettings["Path"], EnvironmentVariableTarget.Process);
			}
			else
			{
				StringBuilder currentPathBuffer = new StringBuilder(maxLength);

				uint length = GetEnvironmentVariable("Path", currentPathBuffer, (uint)maxLength);
				string newPath;
				if (length > 0)
				{
					if (currentPathBuffer.ToString().IndexOf(path) != -1)
						return;

					newPath = path + ";" + currentPathBuffer.ToString();
					if (newPath.Length >= maxLength)
						throw new ApplicationException("Can not add to 'Path' environment variable because the resulting value would be too long.");
				}
				else
					newPath = path;

				bool success = SetEnvironmentVariable("Path", newPath);
				if (!success)
					throw new ApplicationException("Could not write to 'Path' environment variable.");
			}
		}

		[DllImport("KERNEL32.dll")]
		private static extern bool SetEnvironmentVariable(string name, string val);

		[DllImport("KERNEL32.dll")]
		private static extern uint GetEnvironmentVariable(string name, StringBuilder valueBuffer, uint bufferSize);
		#endregion

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Initialize(int init);

		[DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AddHook(int lHookId, int lHookType, HookFunction lpfnHook);

		[DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AddHook(int lHookId, int lHookType, DoumentHookFunction lpfnHook);

		[DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AddHook(int lHookId, int lHookType, GenericHookFunction lpfnHook);

		public enum HookActions : int
		{
			AAHOOK_SUCCESS = 0,
			AAHOOK_ERROR = 1,
			AAHOOK_CALL_NEXT_IN_CHAIN = 2,
			AAHOOK_CALL_DEFAULT = 3
		}

		public enum HookIdentifiers : int
		{
			AADMSHOOK_FIRST = 1002,
			AAHOOK_LOGIN = (AADMSHOOK_FIRST + 0),
			AAHOOK_LOGOUT = (AADMSHOOK_FIRST + 1),
			AAHOOK_CREATE_PROJECT = (AADMSHOOK_FIRST + 100),
			AAHOOK_MOVE_PROJECT = (AADMSHOOK_FIRST + 101),
			AAHOOK_DELETE_PROJECT = (AADMSHOOK_FIRST + 102),
			AAHOOK_MODIFY_PROJECT = (AADMSHOOK_FIRST + 103),
			AAHOOK_PROJECT_WORKFLOW = (AADMSHOOK_FIRST + 104),
			AAHOOK_CHECKOUT_PROJECT = (AADMSHOOK_FIRST + 105),
			AAHOOK_COPYOUT_PROJECT = (AADMSHOOK_FIRST + 106),
			AAHOOK_PURGE_PROJECT = (AADMSHOOK_FIRST + 107),
			AAHOOK_EXPORT_PROJECT = (AADMSHOOK_FIRST + 108),
			AAHOOK_UPGRADE_PROJECT_TO_RICHPRJ = (AADMSHOOK_FIRST + 109),
			AAHOOK_DOWNGRADE_PROJECT_TO_FOLDER = (AADMSHOOK_FIRST + 110),
			AAHOOK_CREATE_DOCUMENT = (AADMSHOOK_FIRST + 200),
			AAHOOK_MOVE_DOCUMENT = (AADMSHOOK_FIRST + 201),
			AAHOOK_DELETE_DOCUMENT = (AADMSHOOK_FIRST + 202),
			AAHOOK_MODIFY_DOCUMENT = (AADMSHOOK_FIRST + 203),
			AAHOOK_CHECKOUT_DOCUMENT = (AADMSHOOK_FIRST + 204),
			AAHOOK_COPYOUT_DOCUMENT = (AADMSHOOK_FIRST + 205),
			AAHOOK_EXPORT_DOCUMENT = (AADMSHOOK_FIRST + 207),
			AAHOOK_CHECKIN_DOCUMENT = (AADMSHOOK_FIRST + 208),
			AAHOOK_PURGE_DOCUMENT_COPY = (AADMSHOOK_FIRST + 209),
			AAHOOK_FREE_DOCUMENT = (AADMSHOOK_FIRST + 210),
			AAHOOK_REFRESH_DOC_SERV_COPY = (AADMSHOOK_FIRST + 211),
			AAHOOK_REFRESH_DOCUMENT_COPY = (AADMSHOOK_FIRST + 212),
			AAHOOK_CHANGE_DOC_VERSION = (AADMSHOOK_FIRST + 213),
			AAHOOK_CHANGE_DOC_STATE = (AADMSHOOK_FIRST + 214),
			AAHOOK_CREATE_REDLINE_DOC = (AADMSHOOK_FIRST + 215),
			AAHOOK_UPDATE_LINK_DATA = (AADMSHOOK_FIRST + 216),
			AAHOOK_DELETE_LINK_DATA = (AADMSHOOK_FIRST + 217),
			AAHOOK_LOCK_DOCUMENT = (AADMSHOOK_FIRST + 218),
			AAHOOK_ADD_DOCUMENT_FILE = (AADMSHOOK_FIRST + 219),
			AAHOOK_DELETE_DOCUMENT_FILE = (AADMSHOOK_FIRST + 220),
			AAHOOK_CHANGE_DOCUMENT_FILE = (AADMSHOOK_FIRST + 221),
			AAHOOK_FETCH_MULTIDOCS = (AADMSHOOK_FIRST + 222),
			AAHOOK_DELETE_DOCUMENT_EXT = (AADMSHOOK_FIRST + 223),
			AAHOOK_DELETE_DOCUMENTS = (AADMSHOOK_FIRST + 224),
			AAHOOK_COPY_DOCUMENT_CROSS_DS = (AADMSHOOK_FIRST + 225),
			AAHOOK_CREATE_SET = (AADMSHOOK_FIRST + 300),
			AAHOOK_ADD_SET_MEMBER = (AADMSHOOK_FIRST + 301),
			AAHOOK_DELETE_SET_MEMBER = (AADMSHOOK_FIRST + 302),
			AAHOOK_VERIFY_VERSION = (AADMSHOOK_FIRST + 400),
			AAHOOK_VERIFY_TABLES = (AADMSHOOK_FIRST + 401),
			AAHOOK_CREATE_TABLES = (AADMSHOOK_FIRST + 402),
			AAHOOK_COPY_DOC_ATTRIBUTES = (AADMSHOOK_FIRST + 500),
			AAHOOK_DELETE_USER = (AADMSHOOK_FIRST + 600),
			AAHOOK_SET_DOC_FINAL_STATUS = (AADMSHOOK_FIRST + 601),
			AAHOOK_DELETE_GROUP = (AADMSHOOK_FIRST + 602),
			AAHOOK_DELETE_WORKFLOW = (AADMSHOOK_FIRST + 603),
			AAHOOK_DELETE_STATE = (AADMSHOOK_FIRST + 604),
			AAHOOK_DEL_WORKFLOW_STATE = (AADMSHOOK_FIRST + 605),
			AAHOOK_DELETE_ENVIRONMENT = (AADMSHOOK_FIRST + 606),
			AAHOOK_INVALIDATE_CACHE = (AADMSHOOK_FIRST + 607),
			AAHOOK_ACTIVATE_INTERFACE = (AADMSHOOK_FIRST + 608),
			AAHOOK_COPY_DOCUMENTS = (AADMSHOOK_FIRST + 700),
			AAHOOK_DELETE_USERLIST = (AADMSHOOK_FIRST + 701),
			AAHOOK_COPY_DOCUMENTS_CROSS_DS = (AADMSHOOK_FIRST + 702),
			AAHOOK_CREATE_VIEW = (AADMSHOOK_FIRST + 715),
			AAHOOK_MODIFY_VIEW = (AADMSHOOK_FIRST + 716),
			AAHOOK_DELETE_VIEW = (AADMSHOOK_FIRST + 717),
			AAHOOK_ENUMERATE_VIEWS = (AADMSHOOK_FIRST + 718),
			AAHOOK_GET_VIEWCOLUMN_NAME = (AADMSHOOK_FIRST + 719),
			AAHOOK_GEN_SETTING_SET_VALUE = (AADMSHOOK_FIRST + 730),
			AAHOOK_USER_SETTING_SET_VALUE = (AADMSHOOK_FIRST + 731),
			AAHOOK_GROUP_MEMBER_CHANGE = (AADMSHOOK_FIRST + 732),
			AADMSHOOK_LAST = (AAHOOK_GROUP_MEMBER_CHANGE),
			AADMSHOOK_LAST_RESERVED = 2000,
			AAWINDMSHOOK_FIRST = 3001,
			AAHOOK_OPEN_DOCUMENT = (AAWINDMSHOOK_FIRST + 0),
			AAHOOK_PRINT_DOCUMENT = (AAWINDMSHOOK_FIRST + 1),
			AAHOOK_START_APPLICATION = (AAWINDMSHOOK_FIRST + 2),
			AAHOOK_DOC_SEND_MAIL = (AAWINDMSHOOK_FIRST + 4),
			AAHOOK_VALIDATE_FILE = (AAWINDMSHOOK_FIRST + 5),
			AAHOOK_LOGIN_DLG = (AAWINDMSHOOK_FIRST + 7),
			AAHOOK_CREATE_PROJECT_DLG = (AAWINDMSHOOK_FIRST + 8),
			AAHOOK_MODIFY_PROJECT_DLG = (AAWINDMSHOOK_FIRST + 9),
			AAHOOK_PROJECT_PROPERTY_DLG = (AAWINDMSHOOK_FIRST + 10),
			AAHOOK_SELECT_PROJECT_DLG = (AAWINDMSHOOK_FIRST + 11),
			AAHOOK_CREATE_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 12),
			AAHOOK_SAVE_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 13),
			AAHOOK_OPEN_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 14),
			AAHOOK_MODIFY_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 15),
			AAHOOK_DOCUMENT_PROPERTY_DLG = (AAWINDMSHOOK_FIRST + 16),
			AAHOOK_SELECT_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 17),
			AAHOOK_FIND_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 19),
			AAHOOK_DOCUMENT_VERSION_DLG = (AAWINDMSHOOK_FIRST + 20),
			AAHOOK_WORKFLOW_DLG = (AAWINDMSHOOK_FIRST + 21),
			AAHOOK_CREATE_SET_DLG = (AAWINDMSHOOK_FIRST + 22),
			AAHOOK_MODIFY_SET_DLG = (AAWINDMSHOOK_FIRST + 23),
			AAHOOK_USER_SETTINGS_DLG = (AAWINDMSHOOK_FIRST + 24),
			AAHOOK_VIEW_EDITOR_DLG = (AAWINDMSHOOK_FIRST + 26),
			AAHOOK_VIEW_DOCUMENTS = (AAWINDMSHOOK_FIRST + 27),
			AAHOOK_VIEW_FILE = (AAWINDMSHOOK_FIRST + 28),
			AAHOOK_CLOSE_VIEWER = (AAWINDMSHOOK_FIRST + 29),
			AAHOOK_SHOW_NOTICE_WND = (AAWINDMSHOOK_FIRST + 30),
			AAHOOK_SPLASH_WINDOW = (AAWINDMSHOOK_FIRST + 31),
			AAHOOK_CREATE_REDLINE_DOC_DLG = (AAWINDMSHOOK_FIRST + 32),
			AAHOOK_SELECT_REDLINE_DOC_DLG = (AAWINDMSHOOK_FIRST + 33),
			AAHOOK_START_REDLINE = (AAWINDMSHOOK_FIRST + 34),
			AAHOOK_REDLINE_FIND_FILE = (AAWINDMSHOOK_FIRST + 35),
			AAHOOK_IMPORTBYDROPHANDLE = (AAWINDMSHOOK_FIRST + 37),
			AAHOOK_EXEC_MENU_COMMAND = (AAWINDMSHOOK_FIRST + 38),
			AAHOOK_INIT_POPUPMENU = (AAWINDMSHOOK_FIRST + 39),
			AAHOOK_SELECT_INTERFACE_DLG = (AAWINDMSHOOK_FIRST + 40),
			AAHOOK_OPEN_DOCUMENT_DLG2 = (AAWINDMSHOOK_FIRST + 42),
			AAHOOK_PROJECT_EXPORT_WZRD = (AAWINDMSHOOK_FIRST + 43),
			AAHOOK_CREATE_DOCUMENTS_DLG = (AAWINDMSHOOK_FIRST + 44),
			AAHOOK_TRANSFER_DOCUMENT_DLG = (AAWINDMSHOOK_FIRST + 45),
			AAHOOK_SELECT_ENVIRONMENT_DLG = (AAWINDMSHOOK_FIRST + 46),
			AAHOOK_CODE_RESERVATION_DLG = (AAWINDMSHOOK_FIRST + 47),
			AAHOOK_DOCUMENT_EXPORT_WZRD = (AAWINDMSHOOK_FIRST + 48),
			AAHOOK_OPEN_DOCUMENTS_DLG2 = (AAWINDMSHOOK_FIRST + 49),
			AAHOOK_SHOW_DOC_PROP_PAGE = (AAWINDMSHOOK_FIRST + 50),
			AAHOOK_SHOW_PROJ_PROP_PAGE = (AAWINDMSHOOK_FIRST + 51),
			AAHOOK_EXECUTE_DOC_ACTION = (AAWINDMSHOOK_FIRST + 52),
			AAHOOK_RELOAD_UPDATED_DOCS_DLG = (AAWINDMSHOOK_FIRST + 53),
			AAHOOK_CODE_GENERATION_DLG = (AAWINDMSHOOK_FIRST + 54),
			AAHOOK_SAVE_DOCUMENT_DLG3 = (AAWINDMSHOOK_FIRST + 55),
			AAHOOK_DOCUMENT_IN_USE_CHECK = (AAWINDMSHOOK_FIRST + 56),
			AAHOOK_IMPORTBYSTGMEDIUM = (AAWINDMSHOOK_FIRST + 57),
			AAHOOK_DLG_APPCHANGED = (AAWINDMSHOOK_FIRST + 58),
			AAHOOK_OPEN_MULTI_DOCUMENTS_DLG = (AAWINDMSHOOK_FIRST + 59),
			AAWINDMSHOOK_LAST = (AAHOOK_OPEN_MULTI_DOCUMENTS_DLG),
			AAWINDMSHOOK_LAST_RESERVED = 4000
		}

		public enum HookTypes : int
		{
			AAPREHOOK = 1,
			AAACTIONHOOK = 2,
			AAPOSTHOOK = 3,
			AAPOSTHOOK_FAIL = 4
		}

		public delegate int HookFunction
		(
			int hookId,
			int hookType,
			int aParam1,
			int aParam2,
			ref int pResult
		 );

		public delegate int DoumentHookFunction
		(
			int hookId,
			int hookType,
			ref AaDocumentsParam aParam1,
			int aParam2,
			ref int pResult
		 );

		public delegate int GenericHookFunction
		(
			int hookId,
			int hookType,
			[In, Out] IntPtr aParam1,
			int aParam2,
			ref int pResult
		);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetProjectNamePath2(int ProjectId, bool UseDesc, char tchSeparator, StringBuilder StringBuffer, int BufferSize);

		public static string GetProjectNamePath(int iProjectId)
		{
			StringBuilder sbPathBuffer = new StringBuilder(5096);
			if (aaApi_GetProjectNamePath2(iProjectId, false, '\\', sbPathBuffer, sbPathBuffer.Capacity))
			{
				return sbPathBuffer.ToString();
			}
			return string.Empty;
		}
	}
}
