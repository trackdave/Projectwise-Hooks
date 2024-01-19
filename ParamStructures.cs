using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Projectwise_Hooks
{
	public static class ParamStructures
	{
		// For AAHOOK_LOGIN
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public unsafe struct _AALOGIN_PARAM
		{
			/// <summary>
			/// [in] Reserved for future use.  Must be 0.
			/// </summary>
			[MarshalAs(UnmanagedType.U4)]
			public UInt32 ulMask;

			/// <summary>
			/// [in] Deprecated. Specifies the data source type. Pass 0.
			/// </summary>
			public int IDsType;

			/// <summary>
			/// [in] Pointer to null-terminated string specifying the data source name.
			/// </summary>
			[MarshalAs(UnmanagedType.LPWStr)]
			public char* lpctstrDSource;

			/// <summary>
			/// [in] Pointer to null-terminated string specifying the user name. This field will be NULL if SSO is used to authenticate the user.
			/// </summary>
			[MarshalAs(UnmanagedType.LPWStr)]
			public char* lpctstrUser;

			/// <summary>
			/// [in] Pointer to null-terminated string specifying the password. This field will be NULL if SSO is used to authenticate the user.
			/// </summary>
			[MarshalAs(UnmanagedType.LPWStr)]
			public char* lpctstrPassword;

			/// <summary>
			/// [in] Deprecated. Specifies the schema file. Pass NULL.
			/// </summary>
			[MarshalAs(UnmanagedType.LPWStr)]
			public char* lpctstrSchema;
		}

		// For AAHOOK_CREATE_PROJECT
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public unsafe struct _AAPROCREATE_PARAM
		{
			// Reserved for future use.  Must be 0.
			[MarshalAs(UnmanagedType.U4)]
			public uint ulMask;

			// Specifies the project Id for the new project.
			public int lProjectId;

			// Specifies the Id of the storage to create the project in. 
			public int lStorageId;

			// Specifies the project owner's account Id.
			public int lManagerId;

			// Specifies new project's type.
			public int lType;

			// Pointer to a null-terminated string specifying the project name.
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lptstrName;

			// Pointer to a null-terminated string specifying the project name.
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lptstrDesc;

			// Pointer to a null-terminated string specifying the project name.
			[MarshalAs(UnmanagedType.LPWStr)]
			public string lptstrConfig;

			// Specifies the parent project identifier.
			public int lParentId;

			// Specifies the source project Id to base the new project on.
			public int lSourceId;

			// Specifies the workflow identifier to be assigned to the new project.
			public int lWorkflowId;

			// Specifies the identifier of the project that access control to copy for the new project.
			public int lAccessProject;

			// Specifies the environment identifier of the project.
			public int lEnvironmentId;

			// Specifies the type of the account assigned as project's owner (member lManagerId).
			public int lManagerType;

			// Specifies the id of workspace profile.
			public int lWorkspaceProfileId;

			// Specifies the pointer to ODS instance object that contains rich project properties.
			public IntPtr pComponentInstance;

			// Specifies the class id of the ODS instance object that contains rich project properties.
			public int lComponentClassId;

			// Specifies the class id of the ODS instance object that contains rich project properties.
			public int lComponentInstanceId;

			// Indicates that a new ODS instance should be created for the rich project properties, and the supplied one is only a template.
			public bool cloneComponentInstance;

			// Specifies valid bits in projFlags.
			[MarshalAs(UnmanagedType.U4)]
			public uint projFlagMask;

			// Specifies the project flags.
			[MarshalAs(UnmanagedType.U4)]
			public uint projFlags;
		}
	}
}
