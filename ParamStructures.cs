using System;
using System.Runtime.InteropServices;

namespace Projectwise_Hooks
{
	public static class ParamStructures
	{
		// For AAHOOK_LOGIN
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public unsafe struct AALOGIN_PARAM
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
		public unsafe struct AAPROCREATE_PARAM
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

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct AaDocParam
		{
			public uint ulMask;
			public int lProjectId;
			public int lDocumentId;
			public int lFileType;
			public int lItemType;
			public int lApplicationId;
			public int lDepartmentId;
			public string lpctstrFileName;
			public string lpctstrName;
			public string lpctstrDes;
			public int lWorkspaceProfileId;
			public Guid docGuid;
			public uint itemFlagMask;
			public uint itemFlags;
			public string pMimeType;
			public string pRevision;
			public string pVersion;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AaDocItem
		{
			public int lProjectId;
			public int lDocumentId;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct AaDocumentsParam
		{
			//  Specifies valid fields mask, specifying whether fields ulFlags and lParam2 are valid or not. 
			// [FieldOffset(0)]
			public uint uiMask;
			//  Specifies count of elements in the array specified by lpDocuments. 
			//[FieldOffset(4)]
			public int iCount;
			//  A Pointer to the array of structures specifying the documents to be processed. 
			//[FieldOffset(8)]
			public AaDocItem lpDocuments;
			//[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
			//public int[] lpDocuments;
			//  Specifies first operation specific parameter. 
			// [FieldOffset(12)]
			public int iParam1;
			//  Specifies operation specific mask. 
			//[FieldOffset(16)]
			public uint uiFlags;
			//  Specifies second operation specific parameter. 
			//[FieldOffset(20)]
			public int iParam2;
			//  Operation comment for audit trail. 
			public string sComment;
			//  Buffer of processed documents (type AADMSBUFFER_DOCUMENT). 
			public IntPtr hProcessedDocuments;
		};
	}
}
