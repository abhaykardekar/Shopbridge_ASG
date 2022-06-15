select * from TBL_PRODUCT_MASTER

select * from tbl_category_master

insert into tbl_category_master
(
category_name
,createdby
,createdon
,modifiedby
,modifiedon
)
values
('FURNITURES','SYS',getdate(),'SYS',getdate())
,('BOOKS','SYS',getdate(),'SYS',getdate())
,('GIFT ITEMS','SYS',getdate(),'SYS',getdate())
,('FASHION','',getdate(),'SYS',getdate())
,('ELECTRONICS','SYS',getdate(),'SYS',getdate())
,('ESSENTIALS','SYS',getdate(),'SYS',getdate())



select * from tbl_sub_category_master

insert into tbl_sub_category_master
(
category_id
,sub_category_name
,createdby
,createdon
,modifiedby
,modifiedon
)
values
(1,'WOOD TABLES','SYS',getdate(),'SYS',getdate())
,(1,'STEEL TABLES','SYS',getdate(),'SYS',getdate())
,(1,'GLASS TABLES','SYS',getdate(),'SYS',getdate())
,(1,'TEAPOY','SYS',getdate(),'SYS',getdate())
,(1,'CUPBOARD','SYS',getdate(),'SYS',getdate())
,(1,'SHELFS','SYS',getdate(),'SYS',getdate())
,(1,'RACKS','SYS',getdate(),'SYS',getdate())
,(2,'NOVELS','SYS',getdate(),'SYS',getdate())
,(2,'STORY-TELLING','SYS',getdate(),'SYS',getdate())
,(3,'FLOWER POT','SYS',getdate(),'SYS',getdate())
,(4,'MENS WEAR','SYS',getdate(),'SYS',getdate())
,(4,'WOMENS WEAR','SYS',getdate(),'SYS',getdate())
,(4,'KIDS WEAR','SYS',getdate(),'SYS',getdate())
,(5,'T.V.','SYS',getdate(),'SYS',getdate())
,(5,'Mobile','SYS',getdate(),'SYS',getdate())
,(5,'Washing Machine','SYS',getdate(),'SYS',getdate())
,(5,'Induction','SYS',getdate(),'SYS',getdate())
,(6,'DRY FRUITS','SYS',getdate(),'SYS',getdate())
,(6,'SHAKES','SYS',getdate(),'SYS',getdate())
