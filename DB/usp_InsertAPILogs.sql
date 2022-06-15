create proc usp_InsertAPILogs
(
@method nvarchar(200)
,@uniquekey nvarchar(200)
,@errorcode nvarchar(20)
,@errordesc nvarchar(max)
,@uniquerefid nvarchar(200)
)
as
begin
	insert into tbl_Shopbridge_APILogs
	(method,uniquekey,uniquerefid,errorcode,errordesc,createdon)
	select @method,@uniquekey,@uniquerefid,@errorcode,@errordesc,getdate()
end