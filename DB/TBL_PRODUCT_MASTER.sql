create table TBL_PRODUCT_MASTER
(
id bigint identity(1,1) primary key,
category_id bigint,
sub_category_id bigint,
product_name nvarchar(200),
product_desc nvarchar(500),
unit_price numeric(10,2),
sell_price numeric(10,2),
discount numeric(10,2),
discount_price numeric(10,2),
product_image_path nvarchar(max),
stock_qty bigint,
createdby nvarchar(200),
createdon datetime,
modifiedby nvarchar(200),
modifiedon datetime
)