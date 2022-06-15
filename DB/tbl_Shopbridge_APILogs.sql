create table tbl_Shopbridge_APILogs
(
requestid bigint identity(1,1)
,method nvarchar(200)
,uniquekey nvarchar(200)
,uniquerefid nvarchar(200)
,errorcode nvarchar(20)
,errordesc nvarchar(max)
,createdon datetime
)