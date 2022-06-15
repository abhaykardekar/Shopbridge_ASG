create table tbl_category_master
(
id bigint identity(1,1) primary key,
category_name nvarchar(300)
,createdby nvarchar(200)
,createdon datetime
,modifiedby nvarchar(200)
,modifiedon datetime
)