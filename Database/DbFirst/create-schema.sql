CREATE TABLE "Suppliers"
(
    "Id" uuid PRIMARY KEY ,
    "Name" varchar(100) NOT NULL ,
    "Country" varchar(50) NOT NULL ,
    "ContactEmail" varchar(100) NOT NULL ,
    "PhoneNumber" varchar(50) NOT NULL ,
    "IsActive" boolean NOT NULL DEFAULT true ,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP ,
    "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP
);


CREATE TABLE "Products"
(
    "Id" uuid PRIMARY KEY ,
    "Name" varchar(100) NOT NULL ,
    "SKU" varchar(50) NOT NULL UNIQUE ,
    "Description" varchar(500) NOT NULL ,
    "Price" double precision NOT NULL ,
    "QuantityInStock" integer NOT NULL ,
    "ExpiryDate" timestamp with time zone NOT NULL ,
    "IsArchived" boolean NOT NULL DEFAULT false ,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP ,
    "LastUpdatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP ,
    "SupplierId" uuid NOT NULL ,

    CONSTRAINT "Products_Price"
        CHECK ("Price" > 0),

    CONSTRAINT "Products_QuantityInStock"
        CHECK ("QuantityInStock" >= 0),

    CONSTRAINT "FK_Products_Suppliers_SupplierId"
        FOREIGN KEY ("SupplierId")
            REFERENCES "Suppliers" ("Id")
            ON DELETE RESTRICT
);

CREATE INDEX "product_supplierId"
    ON "Products" ("SupplierId");


CREATE TABLE "ProductImages"
(
    "Id" uuid PRIMARY KEY ,
    "ProductId" uuid NOT NULL ,
    "FileName" varchar(100) NOT NULL ,
    "FilePath" varchar(100) NOT NULL ,

    CONSTRAINT "FK_ProductImages_Product_ProductId"
        FOREIGN KEY ("ProductId")
            REFERENCES "Products" ("Id")
            ON DELETE NO ACTION
);