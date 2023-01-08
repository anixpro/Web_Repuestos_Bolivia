CREATE DATABASE skberge
GO
USE skberge
GO 

--PROCEDIMIENTOS DEL USUARIO(PERSONA)
	CREATE PROCEDURE sp_loguin @usuario VARCHAR(50), @contrasena VARCHAR(20)
	AS
	BEGIN
	SELECT persona.nombre,personaPermisos.cargo,persona.contrasena, SUBSTRING(nombre,0,5)+SUBSTRING(CAST(persona.rut as VARCHAR(8)),5,4),persona.rut
	FROM persona, personaPermisos
	WHERE persona.usuario =@usuario
	AND persona.contrasena=@contrasena
	AND persona.rut = personaPermisos.rut
	AND habilitado=1;
	END
	GO


	--EL SIGUIENTE PROCEDIMIENTO INSERTA UNA PERSONA Y LE OTROGA PERMISOS
	CREATE PROCEDURE sp_insertaPersona(@empresa VARCHAR(50),@shipCode VARCHAR(50),@rut INTEGER,@nombre VARCHAR(50),@email VARCHAR(50),@telefono VARCHAR(50),@cargo INTEGER,@multipleSucursal BIT)
	AS
	BEGIN
	BEGIN TRY
		BEGIN TRAN
	   
		  DECLARE @descripcion VARCHAR(255);
		  DECLARE @nombreConcesionario VARCHAR(255);
		  DECLARE @nombreEmpresa VARCHAR(255);
		  DECLARE @codSucursal VARCHAR(25);  
		  
		  IF @cargo=1 BEGIN SET @descripcion =  'administrador,administrador,administra todo el sitio, el administrador se encuentra en casa central skberge' SET @codSucursal='IBC05' END
		  ELSE IF @cargo=2 BEGIN SET @descripcion =  'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO' SET @codSucursal=@shipCode END
		  ELSE IF @cargo=3 BEGIN SET @descripcion =  'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios' SET @codSucursal=@shipCode END 
		  ELSE IF @cargo=4 BEGIN SET @descripcion =  'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio' SET @nombreConcesionario='cotizadores' SET @nombreEmpresa=@empresa SET @codSucursal=@shipCode END
		
		  IF EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado)VALUES(@codSucursal,@rut,@nombre,@rut,SUBSTRING(@nombre,0,5)+SUBSTRING(CAST(@rut as VARCHAR(8)),5,4),@email,@telefono,@multipleSucursal,1); INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,@cargo,@descripcion)	 
		  END
		  ELSE IF NOT EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			INSERT INTO sucursal(nombreConcesionario,nombreComuna,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES(@nombreConcesionario,'Santiago',@nombreEmpresa,'','','',@codSucursal,@codSucursal); INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado)VALUES(@codSucursal,@rut,@nombre,@rut,SUBSTRING(@nombre,0,5)+SUBSTRING(CAST(@rut as VARCHAR(8)),5,4),@email,@telefono,@multipleSucursal,1); INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,@cargo,@descripcion);
		  END
		COMMIT TRAN
	END TRY
	BEGIN CATCH
	   ROLLBACK TRAN
	   RAISERROR('se ha producido un error,incosistencia de datos o Puede que se esten duplicando datos', 16, 1);
	END CATCH
	END --FIN STOREPROCEDURE INSERTAPERSONA
	GO
	
	--EL SIGUIENTE PROCEDIMIENTO OTORGA DOBLE PERMISO A UNA PERSONA SOLO QUE POSEA PERMISOS GERENTE U OPERARIO
	CREATE PROCEDURE sp_doblePermiso(@rut INT)
	AS 
	BEGIN
		BEGIN TRY
			BEGIN TRAN
				IF (SELECT personaPermisos.cargo 
					FROM persona,personaPermisos
					WHERE persona.rut= personaPermisos.rut AND persona.habilitado=1
					AND persona.rut =@rut)=3--FIN CONDICION
				BEGIN
				INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,2,'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO')
				END--FIN IF
				ELSE IF(SELECT personaPermisos.cargo 
						FROM persona,personaPermisos
						WHERE persona.rut= personaPermisos.rut AND persona.habilitado=1
						AND persona.rut =@rut)=2--FIN CONDICION
						BEGIN
						INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,3,'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios')
						END--FIN ELSE IF			
			COMMIT TRAN
		END TRY--FIN TRY
		
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('error en sp, puede que el rut asociado ya tenga 2 permisos',1,1)
			
		END CATCH--FIN CATCH
	END--FIN PROCEDIMIENTO
	GO

	
	--EL SIGUIENTE PROCEDIMIENTO ELMINA EL PERMISO DE UNA PERSONA(RUT) ESPECIFICA
	CREATE PROCEDURE sp_eliminaPermiso(@rut INT, @permiso INT)
	AS
	BEGIN
		BEGIN TRY
			BEGIN TRAN
			DELETE FROM personaPermisos WHERE rut=@rut AND cargo=@permiso
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('Error en sp',1,1)
		END CATCH
	END
	GO
	--FIN sp_eliminaPermiso

	CREATE PROCEDURE sp_eliminaPersona(@rut INT)
	AS
	BEGIN
	UPDATE persona SET habilitado=0 WHERE rut=@rut;
	END
	GO
	
	CREATE PROCEDURE sp_rehacerPersona(@rut INT)
	AS
	BEGIN
	UPDATE persona SET habilitado=1 WHERE rut=@rut;
	END
	GO


	CREATE PROCEDURE sp_modificaPersona(@shipCode VARCHAR(50),@rut INTEGER,@email VARCHAR(50),@telefono VARCHAR(50))
	AS
	BEGIN
	UPDATE persona  SET shipCode=@shipCode,email=@email,telefono=@telefono WHERE rut=@rut;
	END
	GO

	CREATE PROCEDURE sp_modificaContrasena(@rut INT,@nuevaContrasena VARCHAR(50))
	AS
	BEGIN
	UPDATE persona  SET contrasena=@nuevaContrasena WHERE rut=@rut;
	END
	GO

	CREATE PROCEDURE sp_reseteaContrasena(@rut INT)
	AS
	BEGIN
	UPDATE persona  SET contrasena=(SELECT SUBSTRING(nombre,0,5) FROM persona WHERE rut=@rut)+SUBSTRING(CAST(@rut as VARCHAR(8)),5,4) WHERE rut=@rut;
	END
	GO
	
	CREATE PROCEDURE sp_saberMiMail(@usuario VARCHAR(50),@contrasena VARCHAR(50))
	AS
	BEGIN
		SELECT email 
		FROM persona
		WHERE usuario=@usuario and contrasena = contrasena;
	END
	GO
	
		CREATE PROCEDURE sp_saberMiNombre(@usuario VARCHAR(50),@contrasena VARCHAR(50))
	AS
	BEGIN
		SELECT nombre 
		FROM persona
		WHERE usuario=@usuario and contrasena = contrasena;
	END
	GO
--PROCEDIMIENTOS DEL USUARIO(PERSONA)

--PROCEDIMIENTOS DE AUTOS
	CREATE PROCEDURE sp_traerConcesionario
	AS
	BEGIN
	SELECT nombreConcesionario 
	FROM concesionario
	WHERE  nombreConcesionario!='cotizadores'
	END
	GO

	CREATE PROCEDURE sp_modificaConcesionario(@nombre VARCHAR(50),@imagen VARCHAR(500))
	AS
	BEGIN
		BEGIN TRY
			BEGIN TRAN
				UPDATE concesionario SET imagen=@imagen WHERE nombreConcesionario=@nombre;
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('Se ha producido un error',1,1);
		END CATCH
	END--FIN STORE PROCEDURE
	GO
	
	CREATE PROCEDURE sp_insertaConcesionario(@sector VARCHAR(10),@nombre VARCHAR(100),@imagen VARCHAR(500),@rutHolding VARCHAR(50), @numeroFactura VARCHAR(50))
	AS
	BEGIN
		BEGIN TRY
			BEGIN TRAN
				--DECLARE @sector INTEGER;
				--IF @sector='norte'BEGIN set @sector=10 END ELSE IF @sector='centro'BEGIN set @sector=11 END ELSE IF @sector='sur'BEGIN set @sector=12 END 
				INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen)VALUES(@sector,@nombre,'consecionario',@imagen, @rutHolding, @numeroFactura)
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('Error en el procedimiento almacenado',1,1)
		END CATCH
	END --FIN DEL PROCEDIMIENTO
	GO
	
	CREATE PROCEDURE sp_insertaSucursal(@nombreConcesionario VARCHAR(50),@nombreComuna VARCHAR(50),@nombreSucursal VARCHAR(50),@organizacionVenta VARCHAR(50),@numeroFactura VARCHAR(50),@rutHolding VARCHAR(50),@shipCode VARCHAR(50),@direccionSucursal VARCHAR(50))
	AS
	BEGIN
		BEGIN TRY
			BEGIN TRAN
				INSERT INTO sucursal(nombreConcesionario,nombreComuna,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES(@nombreConcesionario,@nombreComuna,@nombreSucursal,'','','',@shipCode,@direccionSucursal)
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('Se ha producido un error',1,1);
		END CATCH
	END--FIN STORE PROCEDURE
	GO
	
	CREATE PROCEDURE sp_marcaRut(@rut INTEGER)
	AS
		BEGIN
			SELECT marca.nombreMarca
			FROM sucursal,persona,concesionario,concesionarioMarca,marca
			WHERE persona.rut=@rut
			AND persona.shipCode=sucursal.shipCode
			AND sucursal.nombreConcesionario=concesionario.nombreConcesionario
			AND concesionario.nombreConcesionario = concesionarioMarca.nombreConcesionario
			AND concesionarioMarca.nombreMarca = marca.nombreMarca
		END
	GO
	
	CREATE PROCEDURE sp_marcaConcesionario(@concesionario VARCHAR(50))
	AS
		BEGIN
			SELECT marca.nombreMarca
			FROM concesionario,concesionarioMarca,marca
			WHERE concesionario.nombreConcesionario=@concesionario
			AND concesionario.nombreConcesionario = concesionarioMarca.nombreConcesionario
			AND concesionarioMarca.nombreMarca = marca.nombreMarca
		END
	GO
	
	CREATE PROCEDURE sp_concesionarioRut(@rut INTEGER)
	AS
	BEGIN
		select concesionario.nombreConcesionario
		from sucursal,persona,concesionario
		where persona.rut=@rut
		and persona.shipCode=sucursal.shipCode
		and sucursal.nombreConcesionario=concesionario.nombreConcesionario
	END
	GO

	--MARCA
		--EL SIGUIENTE PROCEDIMIENTO INSERTA UNA NUEVA MARCA AL SISTEMA
		CREATE PROCEDURE sp_insertaMarca(@nombreMarca VARCHAR(30),@abreviado VARCHAR(100),@descripcion VARCHAR(100),@orgVentas VARCHAR(15))
		AS
		BEGIN
			BEGIN TRY
				BEGIN TRAN
					--DECLARE @sector INTEGER;
					--IF @sector='norte'BEGIN set @sector=10 END ELSE IF @sector='centro'BEGIN set @sector=11 END ELSE IF @sector='sur'BEGIN set @sector=12 END 
					INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES(@nombreMarca,@abreviado,@descripcion,@orgVentas)
				COMMIT TRAN
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN
				RAISERROR('Error en el procedimiento almacenado',1,1)
			END CATCH
		END --FIN DEL PROCEDIMIENTO
		GO
		
		
		--EL SIGUIENTE PROCEDIMIENTO ELIMINA UNA MARCA DEL SISTEMA
		CREATE PROCEDURE sp_eliminaMarca(@nombreMarca VARCHAR(30))
		AS
		BEGIN
			BEGIN TRY
				BEGIN TRAN
					--DECLARE @sector INTEGER;
					--IF @sector='norte'BEGIN set @sector=10 END ELSE IF @sector='centro'BEGIN set @sector=11 END ELSE IF @sector='sur'BEGIN set @sector=12 END 
					DELETE FROM marca WHERE nombreMarca=@nombreMarca
				COMMIT TRAN
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN
				RAISERROR('Error en el procedimiento almacenado',1,1)
			END CATCH
		END --FIN DEL PROCEDIMIENTO
		GO
	--MARCA
	
	
	--MODELO
		--EL SIGUIENTE PROCEDIMIENTO ELIMINA UN MODELO DEL SISTEMA
		CREATE PROCEDURE sp_eliminaModelo(@nombreModelo VARCHAR(30))
		AS
		BEGIN
			BEGIN TRY
				BEGIN TRAN
					--DECLARE @sector INTEGER;
					--IF @sector='norte'BEGIN set @sector=10 END ELSE IF @sector='centro'BEGIN set @sector=11 END ELSE IF @sector='sur'BEGIN set @sector=12 END 
					DELETE FROM modelo WHERE nombreModelo =@nombreModelo 
				COMMIT TRAN
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN
				RAISERROR('Error en el procedimiento almacenado',1,1)
			END CATCH
		END --FIN DEL PROCEDIMIENTO
		GO
		
		--EL SIGUIENTE PROCEDIMIENTO INSERTA UN NUEVO MODELO AL SISTEMA
		CREATE PROCEDURE sp_insertaModelo(@nombreMarca VARCHAR(30),@nombreModelo VARCHAR(100),@descripcion VARCHAR(100))
		AS
		BEGIN
			BEGIN TRY
				BEGIN TRAN
					INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES(@nombreMarca,@nombreModelo,@descripcion)
				COMMIT TRAN
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN
				RAISERROR('Error en el procedimiento almacenado',1,1)
			END CATCH
		END --FIN DEL PROCEDIMIENTO
		GO
	--MODELO
--PROCEDIMIENTOS DE AUTOS

--PROCEDIMIENTOS PARA EXCEL
CREATE PROCEDURE sp_insertaIndicador(@idConcesionarioMarca INTEGER,@ano INTEGER,@mes INTEGER,@compras VARCHAR(50),@metas VARCHAR(50))
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			INSERT INTO indicador(idConcesionarioMarca,ano,mes,compras,metas)VALUES(@idConcesionarioMarca,@ano,@mes ,replace(@compras,'.',''),replace(@metas ,'.',''));
		COMMIT TRAN 
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RAISERROR('Error en el procedimiento',1,1)
	END CATCH
END
GO

CREATE PROCEDURE sp_primerTrimestre(@marca VARCHAR(50),@concesionario VARCHAR(50))
AS
BEGIN
	SELECT SUM(indicador.compras)
	FROM  concesionarioMarca,indicador 
	WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	AND concesionarioMarca.nombreMarca=LTRIM(@marca)
	AND concesionarioMarca.nombreConcesionario=LTRIM(@concesionario)
	AND indicador.mes<4
END
GO

CREATE PROCEDURE sp_segundoTrimestre(@marca VARCHAR(50),@concesionario VARCHAR(50))
AS
BEGIN
	SELECT SUM(indicador.compras)
	FROM  concesionarioMarca,indicador 
	WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	AND concesionarioMarca.nombreMarca=LTRIM(@marca)
	AND concesionarioMarca.nombreConcesionario=LTRIM(@concesionario)
	AND indicador.mes between 4 and 6
END
GO

CREATE PROCEDURE sp_tercerTrimestre(@marca VARCHAR(50),@concesionario VARCHAR(50))
AS
BEGIN
	SELECT SUM(indicador.compras)
	FROM  concesionarioMarca,indicador 
	WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	AND concesionarioMarca.nombreMarca=LTRIM(@marca)
	AND concesionarioMarca.nombreConcesionario=LTRIM(@concesionario)
	AND indicador.mes between 7 and 9
END
GO

CREATE PROCEDURE sp_cuartoTrimestre(@marca VARCHAR(50),@concesionario VARCHAR(50))
AS
BEGIN
	SELECT SUM(indicador.compras)
	FROM  concesionarioMarca,indicador 
	WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
	AND concesionarioMarca.nombreMarca=LTRIM(@marca)
	AND concesionarioMarca.nombreConcesionario=LTRIM(@concesionario)
	AND indicador.mes between 10 and 12
END
GO
--PROCEDIMIENTOS PARA EXCEL

--PROCEDIMIENTOS PARA ZONA
CREATE PROCEDURE sp_insertaZona(@sector VARCHAR(50))
AS
BEGIN 
	BEGIN TRY
		BEGIN TRAN
			INSERT INTO zona(sector)VALUES(@sector)
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		RAISERROR('Error en el procedimiento almacenado',1,1)
	END CATCH
END
GO
--PROCEDIMIENTOS PARA ZONA

CREATE TABLE zona (
	idZona INTEGER  NOT NULL IDENTITY(10,1),
	sector VARCHAR(50) NOT NULL UNIQUE,
	PRIMARY KEY(idZona) 
);

CREATE TABLE region(
	idRegion INT IDENTITY(100,1) NOT NULL ,
	nombreRegion varchar(100) NOT NULL UNIQUE, 
	PRIMARY KEY(idRegion)
);

CREATE TABLE provincia(
	idProvincia INT IDENTITY(100,1)  NOT NULL ,
	nombreProvincia varchar(100) NOT NULL UNIQUE,
	nombreRegion varchar(100) NOT NULL,
	PRIMARY KEY(idProvincia),
	FOREIGN KEY(nombreRegion) 
	REFERENCES region(nombreRegion)
	ON DELETE CASCADE
	ON UPDATE CASCADE
);

CREATE TABLE comuna(
	idComuna INT IDENTITY(100,1)  NOT NULL ,
	nombreComuna varchar(100) NOT NULL UNIQUE,
	nombreProvincia varchar(100) NOT NULL,
	PRIMARY KEY(idComuna),
	FOREIGN KEY(nombreProvincia) 
	REFERENCES provincia(nombreProvincia)
	ON DELETE CASCADE
	ON UPDATE CASCADE
);




/*
select*from PROVINCIA

select*from COMUNA

select*from REGION

SELECT PROVINCIA.pro_nom 
FROM PROVINCIA, REGION
WHERE REGION.REG_NOM ='Region Metropolitana de Santiago'
AND REGION.REG_ID=PROVINCIA.PRO_REG_ID

SELECT COMUNA.COM_NOM
FROM COMUNA,PROVINCIA
WHERE PROVINCIA.PRO_NOM='Santiago'
AND PROVINCIA.PRO_ID=COMUNA.COM_PRO_ID
*/
CREATE TABLE marca (
  idMarca INTEGER  NOT NULL IDENTITY(100,1),
  nombreMarca VARCHAR(50) NOT NULL UNIQUE,
  abreviado varchar(10) NOT NULL,
  descripcion VARCHAR(100)NOT NULL,
  orgVentas VARCHAR(10) NOT NULL,
  PRIMARY KEY(idMarca)
);

--drop table marca


CREATE TABLE modelo (
  idModelo INTEGER  NOT NULL IDENTITY(100,1),
  nombreMarca VARCHAR(50) NOT NULL ,
  nombreModelo VARCHAR(50) NOT NULL UNIQUE ,
  descripcion VARCHAR(100)NOT NULL,
  PRIMARY KEY(idModelo),
  FOREIGN KEY (nombreMarca)
  REFERENCES marca(nombreMarca)
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TABLE contenido (
  idContenido INTEGER NOT NULL IDENTITY(1000,1),
  fecha DATE NOT NULL,
  html VARCHAR(8000) NOT NULL,
  titulo VARCHAR(50) NOT NULL,
  tipo INTEGER NOT NULL,
  importante TINYINT NULL DEFAULT 0,
  PRIMARY KEY(idContenido)
);


create table contenido_marca(
	idContenido INTEGER NULL,
	nombreMarca VARCHAR(50) NULL
);

create table contenido_persona(
	idContenido INTEGER NULL,
	idPersona INTEGER NULL
);
CREATE TABLE accesorio (
  idAccesorio INTEGER NOT NULL IDENTITY(1000,1),
  nombreMarca VARCHAR(50) NOT NULL,
  nombreModelo VARCHAR(50) NOT NULL,	
  codigo VARCHAR(100) NOT NULL,
  fecha DATETIME NOT NULL,
  descripcion VARCHAR(1000) NOT NULL,
  imagen VARCHAR(500) NOT NULL,
  PRIMARY KEY(idAccesorio),
  FOREIGN KEY(nombreMarca)
    REFERENCES marca(nombreMarca)
      ON DELETE CASCADE
      ON UPDATE CASCADE
   
);


CREATE TABLE concesionario (
  idConcesionario INTEGER  NOT NULL IDENTITY(1000,1),
  sector VARCHAR(50) NOT NULL,
  nombreConcesionario VARCHAR(50) NOT NULL UNIQUE,
  tipo VARCHAR(50) NULL,
  imagen VARCHAR(500) NOT NULL,
  rutHolding VARCHAR(50) NULL,
  numeroFactura VARCHAR(50) NULL
  PRIMARY KEY(idConcesionario),
  FOREIGN KEY(sector)
    REFERENCES zona(sector)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);
/*CREATE TABLE sucursal (
  idSucursal INTEGER NOT NULL IDENTITY(1000,1),
  nombreConcesionario VARCHAR(50) NOT NULL,
  nombreSucursal VARCHAR(50) NOT NULL,
  shipCode VARCHAR(50) NOT NULL UNIQUE,
  direccionSucursal VARCHAR(100) NOT NULL UNIQUE,
  regionSucursal VARCHAR(50) NOT NULL,
  provinciaSucursal VARCHAR(75) NOT NULL,
  nombreComuna VARCHAR(50) NOT NULL,
  PRIMARY KEY(idSucursal),
  FOREIGN KEY(nombreConcesionario)
    REFERENCES concesionario(nombreConcesionario)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);*/

CREATE TABLE concesionarioMarca (
	idConcesionarioMarca INTEGER  NOT NULL IDENTITY(100,1),
	nombreMarca VARCHAR(50) NOT NULL,
	nombreConcesionario VARCHAR(50) NOT NULL,
	PRIMARY KEY(idConcesionarioMarca),
	FOREIGN KEY(nombreConcesionario)
    REFERENCES concesionario(nombreConcesionario)
      ON DELETE CASCADE
      ON UPDATE CASCADE,
	  FOREIGN KEY(nombreMarca)
    REFERENCES marca(nombreMarca)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);

CREATE TABLE linksInteres(
	idLinks integer not null identity(100,1),
	url varchar(500) not null,
	descripcion varchar(500),
	fechaCreacion datetime,
	primary key(idLinks)

);

CREATE TABLE sucursal (
  idSucursal INTEGER NOT NULL IDENTITY(1000,1),
  nombreConcesionario VARCHAR(50) NOT NULL,
  nombreComuna VARCHAR(100) NOT NULL,
  nombreSucursal VARCHAR(50) NOT NULL,
  organizacionVenta VARCHAR(50) NOT NULL,
  numeroFactura VARCHAR(50) NOT NULL,
  rutHolding VARCHAR(50) NOT NULL,
  shipCode VARCHAR(50) NOT NULL UNIQUE,
  direccionSucursal VARCHAR(100) NOT NULL ,
  PRIMARY KEY(idSucursal),
  FOREIGN KEY(nombreConcesionario)
  REFERENCES concesionario(nombreConcesionario)
  ON DELETE CASCADE
  ON UPDATE CASCADE,
  FOREIGN KEY(nombreComuna)
  REFERENCES comuna(nombreComuna)
  ON DELETE CASCADE
  ON UPDATE CASCADE
  );


CREATE TABLE persona (
  idPersona INTEGER NOT NULL IDENTITY(1000,1),
  shipCode VARCHAR(50) NOT NULL,
  rut INTEGER NOT NULL UNIQUE,
  nombre VARCHAR(50) NOT NULL,
  usuario VARCHAR(50) NOT NULL,
  contrasena VARCHAR(50) NOT NULL,
  email VARCHAR(50) NOT NULL,
  telefono VARCHAR(50) NOT NULL,
  multipleSucursal BIT NOT NULL,
  habilitado BIT NOT NULL,
  PRIMARY KEY(idPersona),
  FOREIGN KEY(shipCode)
    REFERENCES sucursal(shipCode)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);

CREATE TABLE personaPermisos (
  idPermisos INTEGER  NOT NULL IDENTITY(1,1),
  rut INTEGER NOT NULL,
  cargo INTEGER NOT NULL,
  descripcion VARCHAR(200) NOT NULL,
  PRIMARY KEY(idPermisos),
	  FOREIGN KEY(rut)
    REFERENCES persona(rut)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);

CREATE TABLE boletines(
	idBoletin INTEGER  NOT NULL IDENTITY(1,1),
	titulo varchar(200) not null,
	descripcion varchar(500) not null,
	fecha datetime not null,
	documento varchar(200) not null,
	PRIMARY KEY(idBoletin)	
);



CREATE TABLE indicador (
  idIndicador INTEGER NOT NULL IDENTITY(10000,1),
  idConcesionarioMarca INTEGER NOT NULL,
  ano INTEGER NOT NULL,
  mes INTEGER NOT NULL,
  compras BIGINT NOT NULL,
  metas BIGINT NOT NULL,
  PRIMARY KEY(idIndicador),
    FOREIGN KEY(idConcesionarioMarca)
    REFERENCES concesionarioMarca(idConcesionarioMarca)
      ON DELETE CASCADE
      ON UPDATE CASCADE
);

/*Tabla encuesta*/
create table encuesta(
	idEncuesta integer not null identity(10, 1),
	pregunta varchar(300) not null,
	texRes1 varchar(300) not null,
	texRes2 varchar(300) not null,
	texRes3 varchar(300) not null,
	texRes4 varchar(300) not null,
	votosR1 integer not null,
	votosR2 integer not null,
	votosR3 integer not null,
	votosR4 integer not null,
	NumOpciones integer not null,
	inicio datetime not null,
	termino datetime not null,
	PRIMARY KEY(idEncuesta)

);


CREATE TABLE LISTA_PRODUCTOS_TEMP(	
	sessionId [varchar](100) NOT NULL,
	marca [varchar](50) NOT NULL,
	codigo [varchar](50) NOT NULL,
	descripcion [varchar](50) NOT NULL,
	grupoMat [varchar](50) NOT NULL,
	stock [varchar](50) NOT NULL,
	precio int NOT NULL,
	precioConce int NOT NULL,
	cantidad int not null,
	total int null,
	totalLista int null,
	unique(codigo)
);


--Tablas para realizar cotizaciones normales, (Generar PDFs)
CREATE TABLE COTIZACION_COMUN(
	idCotizacionComun INT IDENTITY(1000,1) NOT NULL,
	idSession varchar(200)not null,
	fechaCreacion datetime not null,
	rutCotizador varchar(10) not null,
	primary key(idCotizacionComun)	
);


CREATE TABLE COTIZACION_COMUN_REPUESTO(
	idCotizacionComun varchar(200) NOT NULL,
	marca [varchar](50) NOT NULL,
	codigo [varchar](50) NOT NULL,
	descripcion [varchar](50) NOT NULL,
	stock [varchar](50) NOT NULL,
	precioConce int NOT NULL,
	precioLista int NOT NULL,
	cantidad int not null,
	total int null,
	totalLista int null
);

--select * from COTIZACION_COMUN
--select * from COTIZACION_COMUN_REPUESTO
--drop table COTIZACION_COMUN
--drop table COTIZACION_COMUN_REPUESTO
--/////////////////////////////////////////////////


--TABLAS BACKORDER/////////////////////////////////////////
CREATE TABLE BACKORDER(
		num_backOrder varchar(50) primary key not null,
		id_pedido varchar(50) not null,
		rut_user varchar(10) not null,
		fecha_creacion datetime not null,
		codigo_rep varchar(50) not null,
		marca varchar(50) not null,
		cantidad int not null,
		detalle_rep varchar(200) not null		
);
--select * from BACKORDER
--delete from BACKORDER
--DROP TABLE BACKORDER_REPUESTO
--//////////////////////////////////////////////////////////


--TABLAS DESCARTADOS/////////////////////////////////////////
CREATE TABLE DESCARTADOS(
		idDescartado int identity(100,1) primary key,
		id_pedido varchar(50) not null,
		rut_user varchar(10) not null,
		fechaDescarte datetime not null,
		codigo_rep varchar(50) not null,
		marca varchar(50) not null,
		cantidad int not null,
		detalle_rep varchar(200) not null		
		);

--TABLA VFC /////////////////////////////////////////
CREATE TABLE VFC(
		num_VFC varchar(50) primary key not null,
		id_pedido varchar(50) not null,
		rut_user varchar(10) not null,
		fecha_creacion datetime not null,
		codigo_rep varchar(50) not null,
		marca varchar(50) not null,
		cantidad int not null,
		detalle_rep varchar(200) not null,
		cod_vin varchar(200) not null		
);



--drop table LISTA_PRODUCTOS_TEMP
--select * from LISTA_PRODUCTOS_TEMP

--select organizacionVenta, nombreSucursal, nombreConcesionario from sucursal where nombreConcesionario = 'CALLEGARI LTDA'
--drop table LISTA_PRODUCTOS_TEMP

create table carro(
	idSession varchar(100) not null,
	marca varchar(50) not null, grupoMat varchar(5) not null,
	codigo varchar(50) not null,
	descripcion varchar(100) not null,
	cantidad int not null, stock int not null,
	precioC int not null,
	totalC int not null,
	precioL int not null,
	totalL int not null
);
--select * from carro
--drop table carro

--SELECT SUM(cantidad * precio) FROM carro WHERE  idSession = 'c3fc75eb-0c0c-4535-a2be-1ac6a002d5fb'
--select * from pedido
--drop table carro
--tablas para cotización
--select * from pedido
--drop table pedido
CREATE TABLE [dbo].[PEDIDO](
	[ID_PEDIDO] [INTEGER] IDENTITY(100,1) PRIMARY KEY NOT NULL,
	[idSession][varchar](100) not null,
	[E_VBELN] [varchar](12) NULL,
	[I_AUART] [varchar](50) NOT NULL,
	[I_BNDDT] [varchar](50) NOT NULL,
	[I_KUNNR] [varchar](20) NOT NULL,
	[I_KUNNR2] [varchar](20) NOT NULL,
	[I_SPART] [varchar](3) NOT NULL,
	[I_TEXTO] [varchar](100) NULL,
	[I_VKORG] [varchar](50) NOT NULL,
	[I_VTWEG] [varchar](50) NOT NULL,
	[SOLICITADO_POR] [varchar](50) NOT NULL,
	[FECHA_SOLICITUD] [datetime] NOT NULL,
	[estado] [varchar](50) NOT NULL,
	[FECHA_EXPIRACION] [datetime] NULL,
	[SUCURSAL] [varchar](50) NOT NULL,
	[TOTAL_NETO] [numeric](18, 0) NOT NULL,
	[COMENTARIOS] [varchar](100) NULL,	
	[E_VBELN_PEDIDO] [varchar](20) NULL,
	[I_AUART_PEDIDO] [varchar](50) NULL,
	[I_LPRIO_PEDIDO] [int] NULL
	);
	go
	
	
	CREATE PROCEDURE sp_FechaExpira(@id_pedido int)
	AS
	BEGIN
		declare @fecha int=0;
		set @fecha=(select cast(FECHA_SOLICITUD as int)+4 from pedido where id_pedido = @id_pedido);
		--print cast (@fecha as datetime)
		update pedido set fecha_expiracion = cast (@fecha as datetime) where id_pedido = @id_pedido
	END
	GO
	
	

CREATE TABLE [dbo].[MATERIALES_PEDIDO](
	[id_pedido] [varchar](50) NOT NULL,
	[marca] [varchar](50) NOT NULL,
	[codigo] [varchar](50) NOT NULL,
	[descripcion] [varchar](max) NOT NULL,
	[strGrupoMateriales] [varchar](50) NOT NULL,
	[cantidad] [int] NOT NULL,
	[stock] [int] NOT NULL,
	[valor] [int] NOT NULL,
	[total][int]not null
);
--drop table MATERIALES_PEDIDO

CREATE TABLE [dbo].[GRUPO_MATERIALES](
	[GRUPO_MATERIAL] [varchar](3) NOT NULL,
	[VKORG] [varchar](4) NOT NULL,
	[MARCA] [varchar](50) NOT NULL,
	[CMPY_CODE] [varchar](2) NULL,
	[PREFIJO] [char](10) NULL,
 CONSTRAINT [PK_GRUPO_MATERIALES] PRIMARY KEY CLUSTERED 
(
	[GRUPO_MATERIAL] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


--insertar datos desde el archivo a la tabla grupo de material
/*BULK
INSERT GRUPO_MATERIALES
FROM 'C:\Users\imatecon03\Desktop\skberge\Desarrollo\FUENTE\SitioSkBerge\App_Data\GRUPO_MATERIALES.txt'
WITH( FIELDTERMINATOR = ',',ROWTERMINATOR = '\n' ) 
GO*/

--Pedido Preferido---------------------------------------------------------
CREATE TABLE PREFERIDO(
			idPreferido[INTEGER] IDENTITY(100,1) PRIMARY KEY NOT NULL,
			idSession varchar(100) not null,
			rutCreador varchar(10) not null,
			fechaCreacion datetime not null,
		    textoDescrip varchar(100) not null,
		    marca varchar(50) not null
			);
CREATE TABLE PREFERIDO_REPUESTO(
		    idPreferido varchar(100) not null,			
			codigo varchar(50) not null,
			descripcion varchar(100) not null,
			grupoMat varchar(10) not null,
			cantidad int not null,
			marca varchar(50) not null		
			);
--select * from PREFERIDO
--select * from PREFERIDO_REPUESTO
--drop table preferido_repuesto
-----------------------------------------------------------------------------------
INSERT INTO zona(sector) values('norte');
INSERT INTO zona(sector) values('centro');
INSERT INTO zona(sector) values('sur');


INSERT INTO REGION (nombreRegion) VALUES('Region de Tarapaca')
INSERT INTO REGION (nombreRegion) VALUES('Region de Antofagasta')
INSERT INTO REGION (nombreRegion) VALUES('Region de Atacama')
INSERT INTO REGION (nombreRegion) VALUES('Region de Coquimbo')
INSERT INTO REGION (nombreRegion) VALUES('Region de Valparaíso')
INSERT INTO REGION (nombreRegion) VALUES('Region del Libertador General Bernardo O"higgins')
INSERT INTO REGION (nombreRegion) VALUES('Region del Maule')
INSERT INTO REGION (nombreRegion) VALUES('Region del Bio-Bio')
INSERT INTO REGION (nombreRegion) VALUES('Region de la Araucanía')
INSERT INTO REGION (nombreRegion) VALUES('Region de los Lagos')
INSERT INTO REGION (nombreRegion) VALUES('Region de Aysen del General Carlos Ibañez del Campo')
INSERT INTO REGION (nombreRegion) VALUES('Region de Magallanes y la Antartica Chilena')
INSERT INTO REGION (nombreRegion) VALUES('Region Metropolitana de Santiago')
INSERT INTO REGION (nombreRegion) VALUES('Region de los Rios')
INSERT INTO REGION (nombreRegion) VALUES('Region de Arica y Parinacota')


INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Iquique','Region de Tarapaca')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Antofagasta','Region de Antofagasta')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('El Loa','Region de Antofagasta')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Tocopilla','Region de Antofagasta')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Copiapó','Region de Atacama')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Chañaral','Region de Atacama')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Huasco','Region de Atacama')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Elqui','Region de Coquimbo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Choapa','Region de Coquimbo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Limarí','Region de Coquimbo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Valparaíso','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Isla de Pascua','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Los Andes','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Petorca','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Quillota','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('San Antonio','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('San Felipe de Aconcagua','Region de Valparaíso')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Cachapoal','Region del Libertador General Bernardo O"higgins')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Cardenal Caro','Region del Libertador General Bernardo O"higgins')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Colchagua','Region del Libertador General Bernardo O"higgins')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Talca','Region del Maule')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Cauquenes','Region del Maule')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Curicó','Region del Maule')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Linares','Region del Maule')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Concepción','Region del Bio-Bio')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Arauco','Region del Bio-Bio')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Biobío','Region del Bio-Bio')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Ñuble','Region del Bio-Bio')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Cautín','Region de la Araucanía')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Malleco','Region de la Araucanía')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Llanquihue','Region de los Lagos')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Chiloé','Region de los Lagos')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Osorno','Region de los Lagos')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Palena','Region de los Lagos')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Coyhaique','Region de Aysen del General Carlos Ibañez del Campo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Aisén','Region de Aysen del General Carlos Ibañez del Campo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Capitán Prat','Region de Aysen del General Carlos Ibañez del Campo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('General Carrera','Region de Aysen del General Carlos Ibañez del Campo')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Magallanes','Region de Magallanes y la Antartica Chilena')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Antártica Chilena','Region de Magallanes y la Antartica Chilena')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Tierra del Fuego','Region de Magallanes y la Antartica Chilena')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Última Esperanza','Region de Magallanes y la Antartica Chilena')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Santiago','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Cordillera','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Chacabuco','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Maipo','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Melipilla','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Talagante','Region Metropolitana de Santiago')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Valdivia','Region de los Rios')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Ranco','Region de los Rios')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Arica','Region de Arica y Parinacota')
INSERT INTO PROVINCIA(nombreProvincia,nombreRegion) VALUES('Parinacota','Region de Arica y Parinacota')



INSERT INTO COMUNA VALUES('Iquique','Iquique')
INSERT INTO COMUNA VALUES('Camiña','Iquique')
INSERT INTO COMUNA VALUES('Colchane','Iquique')
INSERT INTO COMUNA VALUES('Huara','Iquique')
INSERT INTO COMUNA VALUES('Pica','Iquique')
INSERT INTO COMUNA VALUES('Pozo Almonte','Iquique')
INSERT INTO COMUNA VALUES('Antofagasta','Antofagasta')
INSERT INTO COMUNA VALUES('Mejillones','Antofagasta')
INSERT INTO COMUNA VALUES('Sierra Gorda','Antofagasta')
INSERT INTO COMUNA VALUES('Taltal','Antofagasta')
INSERT INTO COMUNA VALUES('Calama','El Loa')
INSERT INTO COMUNA VALUES('Ollagüe','El Loa')
INSERT INTO COMUNA VALUES('San Pedro de Atacama','El Loa')
INSERT INTO COMUNA VALUES('Tocopilla','Tocopilla')
INSERT INTO COMUNA VALUES('María Elena','Tocopilla')
INSERT INTO COMUNA VALUES('Copiapó','Copiapó')
INSERT INTO COMUNA VALUES('Caldera','Copiapó')
INSERT INTO COMUNA VALUES('Tierra Amarilla','Copiapó')
INSERT INTO COMUNA VALUES('Chañaral','Chañaral')
INSERT INTO COMUNA VALUES('Diego de Almagro','Chañaral')
INSERT INTO COMUNA VALUES('Vallenar','Huasco')
INSERT INTO COMUNA VALUES('Alto del Carmen','Huasco')
INSERT INTO COMUNA VALUES('Freirina','Huasco')
INSERT INTO COMUNA VALUES('Huasco','Huasco')
INSERT INTO COMUNA VALUES('La Serena','Elqui')
INSERT INTO COMUNA VALUES('Coquimbo','Elqui')
INSERT INTO COMUNA VALUES('Andacollo','Elqui')
INSERT INTO COMUNA VALUES('La Higuera','Elqui')
INSERT INTO COMUNA VALUES('Paiguano','Elqui')
INSERT INTO COMUNA VALUES('Vicuña','Elqui')
INSERT INTO COMUNA VALUES('Illapel','Choapa')
INSERT INTO COMUNA VALUES('Canela','Choapa')
INSERT INTO COMUNA VALUES('Los Vilos','Choapa')
INSERT INTO COMUNA VALUES('Salamanca','Choapa')
INSERT INTO COMUNA VALUES('Ovalle','Limarí')
INSERT INTO COMUNA VALUES('Combarbalá','Limarí')
INSERT INTO COMUNA VALUES('Monte Patria','Limarí')
INSERT INTO COMUNA VALUES('Punitaqui','Limarí')
INSERT INTO COMUNA VALUES('Río Hurtado','Limarí')
INSERT INTO COMUNA VALUES('Valparaíso','Valparaíso')
INSERT INTO COMUNA VALUES('Casablanca','Valparaíso')
INSERT INTO COMUNA VALUES('Concón','Valparaíso')
INSERT INTO COMUNA VALUES('Juan Fernández','Valparaíso')
INSERT INTO COMUNA VALUES('Puchuncaví','Valparaíso')
INSERT INTO COMUNA VALUES('Quilpué','Valparaíso')
INSERT INTO COMUNA VALUES('Quintero','Valparaíso')
INSERT INTO COMUNA VALUES('Villa Alemana','Valparaíso')
INSERT INTO COMUNA VALUES('Viña del Mar','Valparaíso')
INSERT INTO COMUNA VALUES('Isla de Pascua','Isla de Pascua')
INSERT INTO COMUNA VALUES('Los Andes','Los Andes')
INSERT INTO COMUNA VALUES('Calle Larga','Los Andes')
INSERT INTO COMUNA VALUES('Rinconada','Los Andes')
INSERT INTO COMUNA VALUES('San Esteban','Los Andes')
INSERT INTO COMUNA VALUES('La Ligua','Petorca')
INSERT INTO COMUNA VALUES('Cabildo','Petorca')
INSERT INTO COMUNA VALUES('Papudo','Petorca')
INSERT INTO COMUNA VALUES('Petorca','Petorca')
INSERT INTO COMUNA VALUES('Zapallar','Petorca')
INSERT INTO COMUNA VALUES('Quillota','Quillota')
INSERT INTO COMUNA VALUES('Calera','Quillota')
INSERT INTO COMUNA VALUES('Hijuelas','Quillota')
INSERT INTO COMUNA VALUES('La Cruz','Quillota')
INSERT INTO COMUNA VALUES('Limache','Quillota')
INSERT INTO COMUNA VALUES('Nogales','Quillota')
INSERT INTO COMUNA VALUES('Olmué','Quillota')
INSERT INTO COMUNA VALUES('San Antonio','San Antonio')
INSERT INTO COMUNA VALUES('Algarrobo','San Antonio')
INSERT INTO COMUNA VALUES('Cartagena','San Antonio')
INSERT INTO COMUNA VALUES('El Quisco','San Antonio')
INSERT INTO COMUNA VALUES('El Tabo','San Antonio')
INSERT INTO COMUNA VALUES('Santo Domingo','San Antonio')
INSERT INTO COMUNA VALUES('San Felipe','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Catemu','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Llaillay','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Panquehue','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Putaendo','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Santa María','San Felipe de Aconcagua')
INSERT INTO COMUNA VALUES('Rancagua','Cachapoal')
INSERT INTO COMUNA VALUES('Codegua','Cachapoal')
INSERT INTO COMUNA VALUES('Coinco','Cachapoal')
INSERT INTO COMUNA VALUES('Coltauco','Cachapoal')
INSERT INTO COMUNA VALUES('Doñihue','Cachapoal')
INSERT INTO COMUNA VALUES('Graneros','Cachapoal')
INSERT INTO COMUNA VALUES('Las Cabras','Cachapoal')
INSERT INTO COMUNA VALUES('Machalí','Cachapoal')
INSERT INTO COMUNA VALUES('Malloa','Cachapoal')
INSERT INTO COMUNA VALUES('Mostazal','Cachapoal')
INSERT INTO COMUNA VALUES('Olivar','Cachapoal')
INSERT INTO COMUNA VALUES('Peumo','Cachapoal')
INSERT INTO COMUNA VALUES('Pichidegua','Cachapoal')
INSERT INTO COMUNA VALUES('Quinta de Tilcoco','Cachapoal')
INSERT INTO COMUNA VALUES('Rengo','Cachapoal')
INSERT INTO COMUNA VALUES('Requínoa','Cachapoal')
INSERT INTO COMUNA VALUES('San Vicente','Cachapoal')
INSERT INTO COMUNA VALUES('Pichilemu','Cardenal Caro')
INSERT INTO COMUNA VALUES('La Estrella','Cardenal Caro')
INSERT INTO COMUNA VALUES('Litueche','Cardenal Caro')
INSERT INTO COMUNA VALUES('Marchihue','Cardenal Caro')
INSERT INTO COMUNA VALUES('Navidad','Cardenal Caro')
INSERT INTO COMUNA VALUES('Paredones','Cardenal Caro')
INSERT INTO COMUNA VALUES('San Fernando','Colchagua')
INSERT INTO COMUNA VALUES('Chépica','Colchagua')
INSERT INTO COMUNA VALUES('Chimbarongo','Colchagua')
INSERT INTO COMUNA VALUES('Lolol','Colchagua')
INSERT INTO COMUNA VALUES('Nancagua','Colchagua')
INSERT INTO COMUNA VALUES('Palmilla','Colchagua')
INSERT INTO COMUNA VALUES('Peralillo','Colchagua')
INSERT INTO COMUNA VALUES('Placilla','Colchagua')
INSERT INTO COMUNA VALUES('Pumanque','Colchagua')
INSERT INTO COMUNA VALUES('Santa Cruz','Colchagua')
INSERT INTO COMUNA VALUES('Talca','Talca')
INSERT INTO COMUNA VALUES('Constitución','Talca')
INSERT INTO COMUNA VALUES('Curepto','Talca')
INSERT INTO COMUNA VALUES('Empedrado','Talca')
INSERT INTO COMUNA VALUES('Maule','Talca')
INSERT INTO COMUNA VALUES('Pelarco','Talca')
INSERT INTO COMUNA VALUES('Pencahue','Talca')
INSERT INTO COMUNA VALUES('Río Claro','Talca')
INSERT INTO COMUNA VALUES('San Clemente','Talca')
INSERT INTO COMUNA VALUES('San Rafael','Talca')
INSERT INTO COMUNA VALUES('Cauquenes','Cauquenes')
INSERT INTO COMUNA VALUES('Chanco','Cauquenes')
INSERT INTO COMUNA VALUES('Pelluhue','Cauquenes')
INSERT INTO COMUNA VALUES('Curicó','Curicó')
INSERT INTO COMUNA VALUES('Hualañé','Curicó')
INSERT INTO COMUNA VALUES('Licantén','Curicó')
INSERT INTO COMUNA VALUES('Molina','Curicó')
INSERT INTO COMUNA VALUES('Rauco','Curicó')
INSERT INTO COMUNA VALUES('Romeral','Curicó')
INSERT INTO COMUNA VALUES('Sagrada Familia','Curicó')
INSERT INTO COMUNA VALUES('Teno','Curicó')
INSERT INTO COMUNA VALUES('Vichuquén','Curicó')
INSERT INTO COMUNA VALUES('Linares','Linares')
INSERT INTO COMUNA VALUES('Colbún','Linares')
INSERT INTO COMUNA VALUES('Longaví','Linares')
INSERT INTO COMUNA VALUES('Parral','Linares')
INSERT INTO COMUNA VALUES('Retiro','Linares')
INSERT INTO COMUNA VALUES('San Javier','Linares')
INSERT INTO COMUNA VALUES('Villa Alegre','Linares')
INSERT INTO COMUNA VALUES('Yerbas Buenas','Linares')
INSERT INTO COMUNA VALUES('Concepción','Concepción')
INSERT INTO COMUNA VALUES('Coronel','Concepción')
INSERT INTO COMUNA VALUES('Chiguayante','Concepción')
INSERT INTO COMUNA VALUES('Florida','Concepción')
INSERT INTO COMUNA VALUES('Hualqui','Concepción')
INSERT INTO COMUNA VALUES('Lota','Concepción')
INSERT INTO COMUNA VALUES('Penco','Concepción')
INSERT INTO COMUNA VALUES('San Pedro de La Paz','Concepción')
INSERT INTO COMUNA VALUES('Santa Juana','Concepción')
INSERT INTO COMUNA VALUES('Talcahuano','Concepción')
INSERT INTO COMUNA VALUES('Tomé','Concepción')
INSERT INTO COMUNA VALUES('Lebu','Arauco')
INSERT INTO COMUNA VALUES('Arauco','Arauco')
INSERT INTO COMUNA VALUES('Cañete','Arauco')
INSERT INTO COMUNA VALUES('Contulmo','Arauco')
INSERT INTO COMUNA VALUES('Curanilahue','Arauco')
INSERT INTO COMUNA VALUES('Los Alamos','Arauco')
INSERT INTO COMUNA VALUES('Tirúa','Arauco')
INSERT INTO COMUNA VALUES('Los Angeles','Biobío')
INSERT INTO COMUNA VALUES('Antuco','Biobío')
INSERT INTO COMUNA VALUES('Cabrero','Biobío')
INSERT INTO COMUNA VALUES('Laja','Biobío')
INSERT INTO COMUNA VALUES('Mulchén','Biobío')
INSERT INTO COMUNA VALUES('Nacimiento','Biobío')
INSERT INTO COMUNA VALUES('Negrete','Biobío')
INSERT INTO COMUNA VALUES('Quilaco','Biobío')
INSERT INTO COMUNA VALUES('Quilleco','Biobío')
INSERT INTO COMUNA VALUES('San Rosendo','Biobío')
INSERT INTO COMUNA VALUES('Santa Bárbara','Biobío')
INSERT INTO COMUNA VALUES('Tucapel','Biobío')
INSERT INTO COMUNA VALUES('Yumbel','Biobío')
INSERT INTO COMUNA VALUES('Chillán','Ñuble')
INSERT INTO COMUNA VALUES('Bulnes','Ñuble')
INSERT INTO COMUNA VALUES('Cobquecura','Ñuble')
INSERT INTO COMUNA VALUES('Coelemu','Ñuble')
INSERT INTO COMUNA VALUES('Coihueco','Ñuble')
INSERT INTO COMUNA VALUES('Chillán Viejo','Ñuble')
INSERT INTO COMUNA VALUES('El Carmen','Ñuble')
INSERT INTO COMUNA VALUES('Ninhue','Ñuble')
INSERT INTO COMUNA VALUES('Ñiquén','Ñuble')
INSERT INTO COMUNA VALUES('Pemuco','Ñuble')
INSERT INTO COMUNA VALUES('Pinto','Ñuble')
INSERT INTO COMUNA VALUES('Portezuelo','Ñuble')
INSERT INTO COMUNA VALUES('Quillón','Ñuble')
INSERT INTO COMUNA VALUES('Quirihue','Ñuble')
INSERT INTO COMUNA VALUES('Ránquil','Ñuble')
INSERT INTO COMUNA VALUES('San Carlos','Ñuble')
INSERT INTO COMUNA VALUES('San Fabián','Ñuble')
INSERT INTO COMUNA VALUES('San Ignacio','Ñuble')
INSERT INTO COMUNA VALUES('San Nicolás','Ñuble')
INSERT INTO COMUNA VALUES('Treguaco','Ñuble')
INSERT INTO COMUNA VALUES('Yungay','Ñuble')
INSERT INTO COMUNA VALUES('Temuco','Cautín')
INSERT INTO COMUNA VALUES('Carahue','Cautín')
INSERT INTO COMUNA VALUES('Cunco','Cautín')
INSERT INTO COMUNA VALUES('Curarrehue','Cautín')
INSERT INTO COMUNA VALUES('Freire','Cautín')
INSERT INTO COMUNA VALUES('Galvarino','Cautín')
INSERT INTO COMUNA VALUES('Gorbea','Cautín')
INSERT INTO COMUNA VALUES('Lautaro','Cautín')
INSERT INTO COMUNA VALUES('Loncoche','Cautín')
INSERT INTO COMUNA VALUES('Melipeuco','Cautín')
INSERT INTO COMUNA VALUES('Nueva Imperial','Cautín')
INSERT INTO COMUNA VALUES('Padre las Casas','Cautín')
INSERT INTO COMUNA VALUES('Perquenco','Cautín')
INSERT INTO COMUNA VALUES('Pitrufquén','Cautín')
INSERT INTO COMUNA VALUES('Pucón','Cautín')
INSERT INTO COMUNA VALUES('Saavedra','Cautín')
INSERT INTO COMUNA VALUES('Teodoro Schmidt','Cautín')
INSERT INTO COMUNA VALUES('Toltén','Cautín')
INSERT INTO COMUNA VALUES('Vilcún','Cautín')
INSERT INTO COMUNA VALUES('Villarrica','Cautín')
INSERT INTO COMUNA VALUES('Angol','Malleco')
INSERT INTO COMUNA VALUES('Collipulli','Malleco')
INSERT INTO COMUNA VALUES('Curacautín','Malleco')
INSERT INTO COMUNA VALUES('Ercilla','Malleco')
INSERT INTO COMUNA VALUES('Lonquimay','Malleco')
INSERT INTO COMUNA VALUES('Los Sauces','Malleco')
INSERT INTO COMUNA VALUES('Lumaco','Malleco')
INSERT INTO COMUNA VALUES('Purén','Malleco')
INSERT INTO COMUNA VALUES('Renaico','Malleco')
INSERT INTO COMUNA VALUES('Traiguén','Malleco')
INSERT INTO COMUNA VALUES('Victoria','Malleco')
INSERT INTO COMUNA VALUES('Puerto Montt','Llanquihue')
INSERT INTO COMUNA VALUES('Calbuco','Llanquihue')
INSERT INTO COMUNA VALUES('Cochamó','Llanquihue')
INSERT INTO COMUNA VALUES('Fresia','Llanquihue')
INSERT INTO COMUNA VALUES('Frutillar','Llanquihue')
INSERT INTO COMUNA VALUES('Los Muermos','Llanquihue')
INSERT INTO COMUNA VALUES('Llanquihue','Llanquihue')
INSERT INTO COMUNA VALUES('Maullín','Llanquihue')
INSERT INTO COMUNA VALUES('Puerto Varas','Llanquihue')
INSERT INTO COMUNA VALUES('Castro','Chiloé')
INSERT INTO COMUNA VALUES('Ancud','Chiloé')
INSERT INTO COMUNA VALUES('Chonchi','Chiloé')
INSERT INTO COMUNA VALUES('Curaco de Vélez','Chiloé')
INSERT INTO COMUNA VALUES('Dalcahue','Chiloé')
INSERT INTO COMUNA VALUES('Puqueldón','Chiloé')
INSERT INTO COMUNA VALUES('Queilén','Chiloé')
INSERT INTO COMUNA VALUES('Quellón','Chiloé')
INSERT INTO COMUNA VALUES('Quemchi','Chiloé')
INSERT INTO COMUNA VALUES('Quinchao','Chiloé')
INSERT INTO COMUNA VALUES('Osorno','Osorno')
INSERT INTO COMUNA VALUES('Puerto Octay','Osorno')
INSERT INTO COMUNA VALUES('Purranque','Osorno')
INSERT INTO COMUNA VALUES('Puyehue','Osorno')
INSERT INTO COMUNA VALUES('Río Negro','Osorno')
INSERT INTO COMUNA VALUES('San Juan de La Costa','Osorno')
INSERT INTO COMUNA VALUES('San Pablo','Osorno')
INSERT INTO COMUNA VALUES('Chaitén','Palena')
INSERT INTO COMUNA VALUES('Futaleufú','Palena')
INSERT INTO COMUNA VALUES('Hualaihué','Palena')
INSERT INTO COMUNA VALUES('Palena','Palena')
INSERT INTO COMUNA VALUES('Coyhaique','Coyhaique')
INSERT INTO COMUNA VALUES('Lago Verde','Coyhaique')
INSERT INTO COMUNA VALUES('Aisén','Aisén')
INSERT INTO COMUNA VALUES('Cisne','Aisén')
INSERT INTO COMUNA VALUES('Guaitecas','Aisén')
INSERT INTO COMUNA VALUES('Cochrane','Capitán Prat')
INSERT INTO COMUNA VALUES('O'''+'Higgins','Capitán Prat')
INSERT INTO COMUNA VALUES('Tortel','Capitán Prat')
INSERT INTO COMUNA VALUES('Chile Chico','General Carrera')
INSERT INTO COMUNA VALUES('Río Ibáñez','General Carrera')
INSERT INTO COMUNA VALUES('Punta Arenas','Magallanes')
INSERT INTO COMUNA VALUES('Laguna Blanca','Magallanes')
INSERT INTO COMUNA VALUES('Río Verde','Magallanes')
INSERT INTO COMUNA VALUES('San Gregorio','Magallanes')
INSERT INTO COMUNA VALUES('Cabo de Horno','Antártica Chilena')
INSERT INTO COMUNA VALUES('Antártica','Antártica Chilena')
INSERT INTO COMUNA VALUES('Porvenir','Tierra del Fuego')
INSERT INTO COMUNA VALUES('Primavera','Tierra del Fuego')
INSERT INTO COMUNA VALUES('Timaukel','Tierra del Fuego')
INSERT INTO COMUNA VALUES('Natales','Última Esperanza')
INSERT INTO COMUNA VALUES('Torres del Paine','Última Esperanza')
INSERT INTO COMUNA VALUES('Santiago','Santiago')
INSERT INTO COMUNA VALUES('Cerrillos','Santiago')
INSERT INTO COMUNA VALUES('Cerro Navia','Santiago')
INSERT INTO COMUNA VALUES('Conchalí','Santiago')
INSERT INTO COMUNA VALUES('El Bosque','Santiago')
INSERT INTO COMUNA VALUES('Estación Central','Santiago')
INSERT INTO COMUNA VALUES('Huechuraba','Santiago')
INSERT INTO COMUNA VALUES('Independencia','Santiago')
INSERT INTO COMUNA VALUES('La Cisterna','Santiago')
INSERT INTO COMUNA VALUES('La Florida','Santiago')
INSERT INTO COMUNA VALUES('La Granja','Santiago')
INSERT INTO COMUNA VALUES('La Pintana','Santiago')
INSERT INTO COMUNA VALUES('La Reina','Santiago')
INSERT INTO COMUNA VALUES('Las Condes','Santiago')
INSERT INTO COMUNA VALUES('Lo Barnechea','Santiago')
INSERT INTO COMUNA VALUES('Lo Espejo','Santiago')
INSERT INTO COMUNA VALUES('Lo Prado','Santiago')
INSERT INTO COMUNA VALUES('Macul','Santiago')
INSERT INTO COMUNA VALUES('Maipú','Santiago')
INSERT INTO COMUNA VALUES('Ñuñoa','Santiago')
INSERT INTO COMUNA VALUES('Pedro Aguirre Cerda','Santiago')
INSERT INTO COMUNA VALUES('Peñalolén','Santiago')
INSERT INTO COMUNA VALUES('Providencia','Santiago')
INSERT INTO COMUNA VALUES('Pudahuel','Santiago')
INSERT INTO COMUNA VALUES('Quilicura','Santiago')
INSERT INTO COMUNA VALUES('Quinta Normal','Santiago')
INSERT INTO COMUNA VALUES('Recoleta','Santiago')
INSERT INTO COMUNA VALUES('Renca','Santiago')
INSERT INTO COMUNA VALUES('San Joaquín','Santiago')
INSERT INTO COMUNA VALUES('San Miguel','Santiago')
INSERT INTO COMUNA VALUES('San Ramón','Santiago')
INSERT INTO COMUNA VALUES('Vitacura','Santiago')
INSERT INTO COMUNA VALUES('San Bernardo','Santiago')
INSERT INTO COMUNA VALUES('Puente Alto','Santiago')
INSERT INTO COMUNA VALUES('Pirque','Cordillera')
INSERT INTO COMUNA VALUES('San José de Maipo','Cordillera')
INSERT INTO COMUNA VALUES('Colina','Chacabuco')
INSERT INTO COMUNA VALUES('Lampa','Chacabuco')
INSERT INTO COMUNA VALUES('Tiltil','Chacabuco')
INSERT INTO COMUNA VALUES('Buin','Maipo')
INSERT INTO COMUNA VALUES('Calera de Tango','Maipo')
INSERT INTO COMUNA VALUES('Paine','Maipo')
INSERT INTO COMUNA VALUES('Melipilla','Melipilla')
INSERT INTO COMUNA VALUES('Alhué','Melipilla')
INSERT INTO COMUNA VALUES('Curacaví','Melipilla')
INSERT INTO COMUNA VALUES('María Pinto','Melipilla')
INSERT INTO COMUNA VALUES('San Pedro','Melipilla')
INSERT INTO COMUNA VALUES('Talagante','Talagante')
INSERT INTO COMUNA VALUES('El Monte','Talagante')
INSERT INTO COMUNA VALUES('Isla de Maipo','Talagante')
INSERT INTO COMUNA VALUES('Padre Hurtado','Talagante')
INSERT INTO COMUNA VALUES('Peñaflor','Talagante')
INSERT INTO COMUNA VALUES('Valdivia','Valdivia')
INSERT INTO COMUNA VALUES('Corral','Valdivia')
INSERT INTO COMUNA VALUES('Futrono V','Valdivia')
INSERT INTO COMUNA VALUES('La Unión','Valdivia')
INSERT INTO COMUNA VALUES('Lago Ranco V','Valdivia')
INSERT INTO COMUNA VALUES('Lanco','Valdivia')
INSERT INTO COMUNA VALUES('Los Lagos','Valdivia')
INSERT INTO COMUNA VALUES('Máfil','Valdivia')
INSERT INTO COMUNA VALUES('Mariquina','Valdivia')
INSERT INTO COMUNA VALUES('Paillaco','Valdivia')
INSERT INTO COMUNA VALUES('Panguipulli','Valdivia')
INSERT INTO COMUNA VALUES('Río Bueno','Valdivia')
INSERT INTO COMUNA VALUES('La Union','Ranco')
INSERT INTO COMUNA VALUES('Futrono R','Ranco')
INSERT INTO COMUNA VALUES('Lago Ranco R','Ranco')
INSERT INTO COMUNA VALUES('Rio Bueno','Ranco')
INSERT INTO COMUNA VALUES('Arica','Arica')
INSERT INTO COMUNA VALUES('Camarones','Arica')
INSERT INTO COMUNA VALUES('Putre','Parinacota')
INSERT INTO COMUNA VALUES('General Lagos','Parinacota')


--ZONA NORTE
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('norte','REPUESTOS EXPRESS','consecionario','../doc/imgAccesorios/imgConcesionarios.jpg','','');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('norte','CARTONI EXPRESS','consecionario','../doc/imgAccesorios/imgConcesionarios.jpg','','');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','RODAR','../doc/imgAccesorios/imgConcesionarios.jpg','79609330-7','232226');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','SOC HNAS CALLEGARI LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76349970-7','200100');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','CALLEGARI LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','84916800-2','232662');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','AUTOMOTRIZ VAL LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','87831800-5','232326');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','CARTONI','../doc/imgAccesorios/imgConcesionarios.jpg','85430500-K','232312');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','HERNANDEZ MOTORES','../doc/imgAccesorios/imgConcesionarios.jpg','94859000-K','232695');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','LE BLANC LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76870910-6','237790');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','EXPOAUTOS LUIS PASTEUR (SERVICIOS)','../doc/imgAccesorios/imgConcesionarios.jpg','78027430-1','232159');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','COMERCIAL EXPOAUTOS','../doc/imgAccesorios/imgConcesionarios.jpg','79603920-5','232634');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','JOSE VERGARA','../doc/imgAccesorios/imgConcesionarios.jpg','81365800-3','234265');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','SK COMERCIAL','../doc/imgAccesorios/imgConcesionarios.jpg','84196300-8','ICC01');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','AUTOSUMMIT','../doc/imgAccesorios/imgConcesionarios.jpg','96924460-8 ','234111');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','JOSE ETEROVIC','../doc/imgAccesorios/imgConcesionarios.jpg','2972644-2','200198');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','COMERCIAL AUTOMOTORA PRIME LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','77456570-1','232125');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','SOCIEDAD TOCORNAL LOS ANDES','../doc/imgAccesorios/imgConcesionarios.jpg','76355250-0','202930');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','RYR PINTO','../doc/imgAccesorios/imgConcesionarios.jpg','78208920-K','234663');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('norte','consecionario','COINVER','../doc/imgAccesorios/imgConcesionarios.jpg','87523300-9','232322');
--ZONA NORTE

--ZONA CENTRO
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','GMB','../doc/imgAccesorios/imgConcesionarios.jpg','76578110-8','200088');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','VEGA ARTUS','../doc/imgAccesorios/imgConcesionarios.jpg','77810800-5','201016');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','AVENTURA MOTORS','../doc/imgAccesorios/imgConcesionarios.jpg','76186070-4','232095');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','ROSSELOT LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','89117600-7','232338');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','AUTOMOTRIZ ROSSELOT SA','../doc/imgAccesorios/imgConcesionarios.jpg','96502140-K','232416');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','RENTAL SA','../doc/imgAccesorios/imgConcesionarios.jpg','78276630-9','232174');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','FELIPE NOGUERA','../doc/imgAccesorios/imgConcesionarios.jpg','78711940-9','235429');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','MARCELO FRONZA','../doc/imgAccesorios/imgConcesionarios.jpg','78098600-K','232164');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','MARCO RATTI','../doc/imgAccesorios/imgConcesionarios.jpg','4942962-2','200094');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','SERVITAL','../doc/imgAccesorios/imgConcesionarios.jpg','96812030-1','232730');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','LA FORESTA','../doc/imgAccesorios/imgConcesionarios.jpg','78232780-1','232170');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','SERVICIO AUTOMOTRIZ LA FORESTA LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','78414610-3','235425');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','SERVICIOS PATRICIO GARCIA Y COMPANIA','../doc/imgAccesorios/imgConcesionarios.jpg','77899910-2','234070');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','PATRICIO GARCIA Y CIA LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','79897420-3','202918');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','PIAMONTE SA','../doc/imgAccesorios/imgConcesionarios.jpg','96642160-6','232457');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','GUILLERMO MORALES LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','96564810-0 ','232444');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','DITALCAR AUTOMOTRIZ SA','../doc/imgAccesorios/imgConcesionarios.jpg','76939200-9','237481');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','COMERCIAL AUTOMOTRIZ SA','../doc/imgAccesorios/imgConcesionarios.jpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','MARCO NEVEU','../doc/imgAccesorios/imgConcesionarios.jpg','78543880-9','234915');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','AUTOMOTRIZ ITALOAMERICANA LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','77152490-7','202927');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','SK BERGE AUTOMOTRIZ','../doc/imgAccesorios/imgConcesionarios.jpg','96861240-9','IBC06');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','SKBERGE LOGISTICA SA','../doc/imgAccesorios/imgConcesionarios.jpg','96928530-4','IBC15');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','COMERCIAL AUTOMOTRIZ SA MOVICENTER','../doc/imgAccesorios/imgConcesionarios.jpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','consecionario','COMERCIAL AUTOMOTRIZ SA PALD','../doc/imgAccesorios/imgConcesionarios.jpg','96928530-4','IBC05');
--ZONA CENTRO

--ZONA SUR
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','GOMA','../doc/imgAccesorios/imgConcesionarios.jpg','79690930-7','232234');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('sur','COMERCO','consecionario','../doc/imgAccesorios/imgConcesionarios.jpg','','');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','AUTOFRANCE','../doc/imgAccesorios/imgConcesionarios.jpg','84807200-1','232661');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','CORDILLERA','../doc/imgAccesorios/imgConcesionarios.jpg','76024383-3','237622');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','AUTOMOTRIZ VARONA','../doc/imgAccesorios/imgConcesionarios.jpg','76101820-5','232093');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','NALLAR','../doc/imgAccesorios/imgConcesionarios.jpg','79834820-5','232246');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','PATAGONIA','../doc/imgAccesorios/imgConcesionarios.jpg','99575100-3','232754');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','SIGLO XXI','../doc/imgAccesorios/imgConcesionarios.jpg','78770630-4','232199');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','NOVICIADO','../doc/imgAccesorios/imgConcesionarios.jpg','96928530-4','IBC15');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','TRANSWORLD','../doc/imgAccesorios/imgConcesionarios.jpg','77781260-2','232147');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','QUILIN','../doc/imgAccesorios/imgConcesionarios.jpg','89318000-1','232343');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','COMERCIAL NOACK LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76247400-K','217429');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','SOCIEDAD AUTOMOTRIZ NOACK LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76253220-4','232096');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','AUTOMOTRIZ NOACK LTDA D AND P','../doc/imgAccesorios/imgConcesionarios.jpg','76256380-0','200060');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','APC SERVICIOS LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76433870-7','232097');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','APC COMERCIAL LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','76632110-0','200132');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','AUTOIMPACTO','../doc/imgAccesorios/imgConcesionarios.jpg','78453310-7','232183');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','DIFOR CHILE SA','../doc/imgAccesorios/imgConcesionarios.jpg','96918300-5','232523');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','JORGE OSORIO','../doc/imgAccesorios/imgConcesionarios.jpg','5362093-0','217236');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','KORNER','../doc/imgAccesorios/imgConcesionarios.jpg','78510990-2','259570');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','PINTURAS AUTOMOTRICES','../doc/imgAccesorios/imgConcesionarios.jpg','78516790-2','234402');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','BAUER','../doc/imgAccesorios/imgConcesionarios.jpg','86178100-3','259309');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','BRUNO FRITSCH','../doc/imgAccesorios/imgConcesionarios.jpg','84807200-1','232661');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','AUTOMOTRIZ CALAFQUEN LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','78993470-3','232215');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','DEL REAL SERVICIOS LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','77135830-6','201009');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','COM AUT CAODING LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','78068830-0','232162');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','JAIME NAVARRETE GARCIA','../doc/imgAccesorios/imgConcesionarios.jpg','14264517-3','201145');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','TECNACO','../doc/imgAccesorios/imgConcesionarios.jpg','84701300-1','233326');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','TULIO HECTOR LOPEZ REYES','../doc/imgAccesorios/imgConcesionarios.jpg','8051617-7','203013');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','CENTRAL AUTOMOTORA','../doc/imgAccesorios/imgConcesionarios.jpg','78805760-1','201211');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','MLADINIC AUTOMOTRIZ LTDA','../doc/imgAccesorios/imgConcesionarios.jpg','89533300-K','232680');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','EMPRENANI','../doc/imgAccesorios/imgConcesionarios.jpg','78795760-9','232627');
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('sur','consecionario','KISHOR BALCHAND SACHANANDANI','../doc/imgAccesorios/imgConcesionarios.jpg','9286849-4','217478');
--ZONA SUR

--CASA CENTRAL
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','casa central','SKBERGE','../doc/imgAccesorios/imgConcesionarios.jpg','X','X');
--CASA CENTRAL

--COTIZADORES
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen) values('centro','cotizadores','cotizadores','../doc/imgAccesorios/imgConcesionarios.jpg');
--COTIZADORES

--SUCURSALES
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Arica','AUTOMOTRIZ VAL LTDA','Automotora Val Ltda.','BCO2-BC04-BC08','232326','87831800-5','232326','Paula Jara Quemada 1231');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Iquique','SK COMERCIAL','SK Comercial','BC02-BC08-BC03-BC04-BC10.BC11-BC13','ICC01','84196300-8','3100000903','Calle Sta. Rosa de Huara 19H, Manzana C, Barrio Industrial');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Antofagasta','COMERCIAL AUTOMOTORA PRIME LTDA','Com. Aut. Prime Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232125','77456570-1','232125','Condell 2707');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Antofagasta','COMERCIAL AUTOMOTORA PRIME LTDA','Com. Aut. Prime Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232125','77456570-1','3100000904','Perez Zujovic 4534 (Mall Plaza)');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Antofagasta','RODAR','Soc. de Repuestos Rodar Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232226','79609330-7','3100000905','Tacna 55');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Antofagasta','AUTOSUMMIT','AUTOSUMMIT','BC03-BC10-BC04-BC11','234111','96924460-8 ','3100001007','Onix 85, Barrio Industrial');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Calama','COMERCIAL AUTOMOTORA PRIME LTDA','COMERCIAL AUTOMOTORA PRIME LTDA.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232125','77456570-1','3100000906','Granaderos 3120');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Calama','COMERCIAL AUTOMOTORA PRIME LTDA','COMERCIAL AUTOMOTORA PRIME LTDA.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232125','77456570-1','3100000908','Granaderos 2625');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Calama','RODAR','Soc. de Repuestos Rodar Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232226','79609330-7','232226','Granaderos 3120');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Calama','RODAR','Soc. de Repuestos Rodar Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232226','79609330-7','3100000910','Av. Argentina 3137');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Calama','AUTOSUMMIT','AUTOSUMMIT','BC03-BC10-BC04-BC11','234111','96924460-8 ','3100001009','Granaderos 3417?');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('copiapó','SOC HNAS CALLEGARI LTDA','Soc. Hnas. Callegari Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','200100','76349970-7','3100000911','"O""Higgins 401"');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('copiapó','CALLEGARI LTDA','Callegari  Ltda.','BC04','232662','84916800-2','3100000913','Ramon Freire 210');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('La Serena','SOC HNAS CALLEGARI LTDA','Soc. Hnas. Callegari Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','200100','76349970-7','200100','Av. Francisco de Aguirre 060');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('La Serena','CALLEGARI LTDA','Callegari  Ltda.','BC04','232662','84916800-2','232662','Cordovez 780');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('La Serena','CALLEGARI LTDA','Callegari  Ltda.','BC04','232662','84916800-2','3100000916','BALMACEDA 1880');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Ovalle','SOC HNAS CALLEGARI LTDA','Soc. Hnas. Callegari Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','200100','76349970-7','3100000918','COVARRUBIAS 340');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Quillota','ROSSELOT LTDA','ROSSELLOT ','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232338','89117600-7','3100000920','Av. 21 de Mayo 281');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Quillota','MARCELO FRONZA','Marcelo Fronza y Cia. Ltda.','BCO8-BC04-BC10-BC03','232164','78098600-K','3100000922','21 de Mayo 570');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Los Andes','SIGLO XXI','Com. Automotriz siglo XXI S.A.','BCO8-BC04-BC10','232199','78770630-4','3100000924','Av. Argentina 1130 ');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Los Andes','SOCIEDAD TOCORNAL LOS ANDES','SOC.TOCORNAL LOS ANDES','BC08','202930','76355250-0','202930','AVARGENTINA 1130  B15');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('San Felipe','RYR PINTO','RyR Pinto','BC02-BC03-BC04-BC08','234663','78208920-K','3100000928','CHACABUCO 211');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('San Felipe','RYR PINTO','RyR Pinto','BC02-BC03-BC04-BC08','234663','78208920-K','234663','CHACABUCO 235');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','CARTONI','Cartoni y Cartoni S.A.','BCO8-BC02-BC03-BC13','232312','85430500-K','232312','Chacabuco 1775');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','CARTONI','Cartoni y Cartoni S.A.','BCO8-BC02-BC03-BC13','232312','85430500-K','3100000930','Arlegui 145');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','AUTOMOTRIZ ROSSELOT SA','Aut. Rosselot S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232416','96502140-K','3100000932','Calle Limache 3847');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','HERNANDEZ MOTORES','Hernandez Motores','BC02-BC10','232695','94859000-K','3100000936','3 1/2 Oriente 1240');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','HERNANDEZ MOTORES','Hernandez Motores','BC02-BC10','232695','94859000-K','232695','15 Norte 1018');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Viña del Mar','HERNANDEZ MOTORES','Hernandez Motores','BC02-BC10','232695','94859000-K','3100000938','3 ORIENTE 1374');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('San Antonio','AUTOMOTRIZ ROSSELOT SA','Automotriz Rosselot S.A.','BC03','232416','96502140-K','3100000940','Barros Luco 2550');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Valparaíso','COINVER','COINVER','BC03','232322','87523300-9','3100000942','VICTORIA 2593  B24');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Valparaíso','CARTONI','Cartoni y Cartoni S.A.','BCO8-BC02-BC03-BC13','232312','85430500-K','3100000944','CALLE LIMACHE 4299 Valparaíso  C42');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Valparaíso','MARCELO FRONZA','Marcelo Fronza y Cia. Ltda.','BC04','232164','78098600-K','3100000946','HONTANEDA 2615');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Valparaíso','MARCELO FRONZA','Marcelo Fronza y Cia. Ltda.','BC04','232164','78098600-K','232164','Victoria 3033');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Valparaíso','HERNANDEZ MOTORES','Hernandez Motores','BC02-BC10','232695','94859000-K','3100000948','Chacabuco 2012');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Vitacura','FELIPE NOGUERA','Felipe Noguera y Cia.Ltda.','BC02','235429','78711940-9','235429','Av. Kennedy 7286');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Quilicura','COMERCIAL AUTOMOTRIZ SA','Comercial Automotriz S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC05','96928530-4','IBC05','Avenida Americo Vespucio 1601');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Huechuraba','COMERCIAL AUTOMOTRIZ SA MOVICENTER','Comercial Automotriz S.A. Movicenter','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC05','96928530-4','3100000965','Av. Americo Vespucio Norte 1155 Local 615');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Lo Barnechea','COMERCIAL AUTOMOTRIZ SA PALD','Comercial Automotriz S.A. PALD','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC05','96928530-4','3100000943','Av. La Dehesa 1993');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Vitacura','AVENTURA MOTORS','Aventura Motors','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232095','76186070-4','3100000967','Vitacura 7408');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Huechuraba','AVENTURA MOTORS','AVENTURA MOTORS HUECHU','','232095','76186070-4','3100001048','SANTA ELENA DE HUECHURABA 1135 - A19');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Las Condes','COMERCIAL EXPOAUTOS','Comercial Expaoutos','BC02','232634','79603920-5','3100000972','Av. Francisco Bilbao 4370');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Vitacura ','EXPOAUTOS LUIS PASTEUR (SERVICIOS)','EXPOAUTOS LUIS PASTEUR (SERVICIOS)','BC02','232159','78027430-1','232159','AV LUIS PASTEUR 6705  A02');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('La Florida','GUILLERMO MORALES LTDA','Guillermo Morales Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232444','96564810-0 ','3100000974','Av. Vicuna Mackenna 1910 Local 7110 (Auto Plaza)');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Macul','GMB','SERVICIO TECNICO GMB MACUL','','200088','76578110-8','3100001044','EXEQUIEL FERNANDEZ 3305 ');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','ROSSELOT LTDA','ROSSELLOT ','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232338','89117600-7','232338','Avda Vicuna Mackenna 1911');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','AUTOMOTRIZ ROSSELOT SA','Automotriz Rosselot S.A.','','232416','96502140-K','232416','Av. Francisco Bilbao 2139');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','APC SERVICIOS LTDA','Apc Servicios Limitada','BC03','232097','76433870-7','3100000998','Av. Vicuna  Mackenna 1341');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','APC COMERCIAL LTDA','Apc Comercial Limitada','BC03','200132','76632110-0','200132','Av. Americo Vespucio 1501 Loc. AP129 y 131 Plaza Oeste');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','DITALCAR AUTOMOTRIZ SA','Ditalcar Automotriz S.A.','BC10','237481','76939200-9','237481','Av. Las Condes 8127 ');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Cerrillos','BRUNO FRITSCH','Bruno Fritsch Limitada','BC10','232661','84807200-1','3100001002','CAMINO MELIPILLA 9160');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Cerrillos','SIGLO XXI','Com. Automotriz siglo XXI S.A.','BC04','232199','78770630-4','232199','PEDRO AGUIRRE CERDA 5007');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Ñuñoa','PIAMONTE SA','Piamonte S.A.','BC03','232457','96642160-6','232457','Irarrazaval 3400');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','LE BLANC LTDA','AUTOMOTORA LE BLANC LTDA','BC02','237790','76870910-6','237790','AV. MATTA   840');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','JOSE VERGARA','JOSE VERGARA','BC02','234265','81365800-3','234265','ROMaN DIAZ  #  1263');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','SOCIEDAD AUTOMOTRIZ NOACK LTDA','SOCIEDAD AUTOMOTRIZ NOACK LTDA','BC04-BC02','232096','76253220-4','232096','EL AGUILUCHO 3398');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','AUTOMOTRIZ NOACK LTDA D AND P','AUTOMOTRIZ NOACK LTDA D&P','BC04-BC02','200060','76256380-0','200060','LOS HERREROS 8828');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','COMERCIAL NOACK LTDA','Comercial Noack Ltda','BC04','217429','76247400-K','217429','EL AGUILUCHO 3380. PROVIDENCIA STGO');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','MARCO NEVEU','MARCO NEVEU Y CIA','BC04','234915','78543880-9','3100001017','ALMIRANTE LATORRE 41');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','JOSE ETEROVIC','JOSE ETEROVIC','BC04','200198','2972644-2','200198','TOCORNAL 655');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Huechuraba','AUTOMOTRIZ ITALOAMERICANA LTDA','AUTOMOTRIZ ITALOAMERICANA LTDA','BC03','202927','77152490-7','202927','Av. Americo Vespucio Norte 1155 Local 609, Movicenter');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Ñuñoa','PATRICIO GARCIA Y CIA LTDA','PATRICIO GARCIA Y CIA LTDA','BC04','202918','79897420-3','202918','Avda. Irarrazaval 1154');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Ñuñoa','SERVICIOS PATRICIO GARCIA Y COMPANIA','Servicios Patricio Gracia Y Compani','BC04','234070','77899910-2','234070','LUIS BELTRAN 2219. Ñuñoa.');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Vitacura','LA FORESTA','La Foresta','BC04','232170','78232780-1','232170','Tabancura 1405     Vitacura');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Vitacura','LA FORESTA','Servicio Automotriz La Foresta Ltda','BC04','235425','78414610-3','235425','TABANCURA N° 1405.');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Macul','SERVITAL','Servital Automotriz S.A.','BC04','232730','96812030-1','232730','Vicuna Mackenna 2545');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Pudahuel ','SK BERGE AUTOMOTRIZ','SK BERGE Automotriz','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC06','96861240-9','3100001033','Avenida Americo Vespucio 1601, Quilicura');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Lampa','SK COMERCIAL','SK COMERCIAL','BC02-BC08-BC03-BC04-BC10.BC11-BC13','ICC01','84196300-8','ICC01','PANAMERICANA NORTE KM 15 12 LAMPA');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Pudahuel','SKBERGE LOGISTICA SA','SKBerge Logistica S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC15','96928530-4','IBC15','CAMINO AL NOVICIADO KM 3  A64');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Pudahuel','NOVICIADO','Noviciado Bodega 2 (Comsa Logistica)','BC02-BC08-BC03-BC04-BC10.BC11-BC13','IBC15','96928530-4','3100001049','CAMINO AL NOVICIADO KM 3  A64');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Cerrillos','QUILIN','QUILIN','Comercial Automotriz Quilin Ltda','232343','89318000-1','232343','Avda Pedro Aguirre Cerda 5037');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Ñuñoa','AUTOSUMMIT','AUTOSUMMIT','BC04-BC08','234111','96924460-8 ','234111','Av. Vicuna Mackenna 1130');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Rancagua','VEGA ARTUS','Vega Artus','BC02-BC08-BC03-BC04-BC10.BC11-BC13','201016','77810800-5','3100000945','Miguel Ramirez 199');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Rancagua','MARCO RATTI','Marco Ratti Mauri','BC04-BC10','200094','4942962-2','200094','Av.L B. O"Higgins 216 ');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Rancagua','AUTOMOTRIZ CALAFQUEN LTDA','Automotriz Calafquen Ltda.','','232215','78993470-3','232215','Avda L B Ohiggins 119');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Rancagua','DEL REAL SERVICIOS LTDA','Del Real Servicios Ltda','BC03-BC08','201009','77135830-6','201009','ALAMEDA 0153 RANCAGUA');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('San Fernando','RENTAL SA','Rental S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232174','78276630-9','3100000951','"Av. Bernardo o""Higgins 379"');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Curicó','COM AUT CAODING LTDA','Com. Aut. Coadig Ltda.','BC02','232162','78068830-0','232162','Yungay 1063');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Curicó','RENTAL SA','Rental S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232174','78276630-9','232174','Av. Manso de Velasco 791');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Talca','JAIME NAVARRETE GARCIA','Jaime Navarrete Garcia','BC04','201145','14264517-3','201145','1SUR 14 Y 15 ORIENTE 2158A TALCA');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('chillán','AUTOIMPACTO','AUTO IMPACTO','BC04','232183','78453310-7','3100000917','CLAUDIO ARRAU 1043  C52');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('chillán','JORGE OSORIO','Jorge Osorio Uribe','BC04','217236','5362093-0','217236','Pedro Aguirre Cerda 28');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Los Angeles','DIFOR CHILE SA','Difor Chile S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232523','96918300-5','3100001030','Longitudinal Sur 509');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','CORDILLERA','Servicios Cordillera Ltda','BC02-BC08-BC03-BC04-BC10.BC11-BC13','237622','76024383-3','237622','AV. ARTURO PRAT 1037 Concepción');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','KORNER','Korner Automotriz Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','259570','78510990-2','233305','Angol 920');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','DIFOR CHILE SA','Difor Chile S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232523','96918300-5','3100000968','camino a Coronel 3455 Sn pedro de la paz');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','DIFOR CHILE SA','Difor Chile S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232523','96918300-5','232523','Manuel Rodriguez 1329');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','AUTOFRANCE','Autofrance Ltda.','BCO2','232661','84807200-1','232661','PRAT 306');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','AUTOFRANCE','Autofrance Ltda.','BCO2','232661','84807200-1','3100000970','"O""Higgins 132"');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','AUTOFRANCE','Autofrance Ltda.','BCO2','232661','84807200-1','3100000971','Avda. Jorge Alessandri 3177 Talcahuano. Local D-101');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','BAUER','Bauer y Cia Ltda','BC03','259309','86178100-3','259309','ANGOL 630');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','BAUER','Bauer y Cia Ltda','BC03','259309','86178100-3','3100000973','Paicavi 1193');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Talcahuano','AUTOFRANCE','Autofrance Ltda. M. Plaza Trebol','BC03','232661','84807200-1','3100000975','Av. Jorge Alessandri 3177 Local AP-110');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Concepción','TECNACO','Tecnaco','BC03','233326','84701300-1','233326','Maipu 971');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Talcahuano','TECNACO','Tecnaco','BC03','233326','84701300-1','3100000941','AV COLON 8579  D72');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Temuco','DIFOR CHILE SA','Difor Chile S.A.','BC04','232523','96918300-5','3100000907','San Martin 886');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Temuco','GOMA','AUTOMTORA GOMA LTDA.','','232234','79690930-7','3100001045','GENERAL MACKENNA 1030 -');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Osorno','NALLAR','Nallar Autos','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232246','79834820-5','232246','Juan Mackenna 1702');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Varas','PATAGONIA','Patagonia Automotriz S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232754','99575100-3','3100000933','Bio-bio 1150');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Frutillar','TULIO HECTOR LOPEZ REYES','Tulio Hector Lopez Reyes','BC04','203013','8051617-7','203013','Vicente Perez Rosales 516 ');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Montt','DIFOR CHILE SA','Difor Chile S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232523','96918300-5','3100000935','Panamericana 161');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Montt','PINTURAS AUTOMOTRICES','Pinturas Automotrices Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','234402','78516790-2','234402','San Andres 60');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Montt','PATAGONIA','Patagonia Automotriz S.A.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232754','99575100-3','232754','Benavente 690');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Montt','PATAGONIA','PATAGONIA AUTOMOTRIZ S.A.','','232754','99575100-3','3100001043','CALLE PILPILCO 530, PARQUE INDUSTRIAL  ');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Puerto Montt','CENTRAL AUTOMOTORA','Central Automotora','BC03-BC02-BC10','201211','78805760-1','3100000939','Antonio Varas 202');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Coyhaique','AUTOMOTRIZ VARONA','Aut. Varona','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232093','76101820-5','232093','Carrera 333');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Coyhaique','AUTOMOTRIZ VARONA','Aut. Varona','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232093','76101820-5','150088','Carrera 333 (Redestinacion)');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','MLADINIC AUTOMOTRIZ LTDA','Mladinic Automotriz Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232680','89533300-K','3100001016','Croacia # 441 Punta Arenas');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','MLADINIC AUTOMOTRIZ LTDA','Mladinic Automotriz Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232680','89533300-K','232680','Croacia 670');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','EMPRENANI','Emprenani','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232627','78795760-9','232627','Errazuriz 853, piso 3 / Av. Principal manzana 4344 zona Franca');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','EMPRENANI','Emprenani','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232627','78795760-9','3100001018','Av. Principal Sitios 43 - 44');
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','TRANSWORLD','Transworld Suply Automotriz Ltda.','BC02-BC08-BC03-BC04-BC10.BC11-BC13','232147','77781260-2','232147','Avda. Bulnes 03545');

INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Punta Arenas','KISHOR BALCHAND SACHANANDANI','Kishor Balchand Sachanandani','','217478','9286849-4','217478','Republica 484 Zona 5');

--SUCURSAL PARA COTIZAR
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','cotizadores','aseguradora magallanes','','','','sta. teresita #4456','sta. teresita #4456');
--SUCURSAL PARA COTIZAR


--CASA CENTRAL
INSERT INTO sucursal(nombreComuna,nombreConcesionario,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES('Santiago','SKBERGE','SKBERGE','X','X','X','X','X')
--CASA CENTRAL

--USUARIOS
INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('X',17707464,'Rodrigo','administrador','administrador','rorro@hotmail.com','1234567',0,1);	
INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('3100000943',11111111,'Jorge','concesionarioG','concesionarioG','jorge@hotmail.com','1234567',0,1);
INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('3100000943',22222222,'Charlie','concesionarioO','concesionarioO','charlie@hotmail.com','1234567',0,1);
INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('217478',33333333,'José','usuarioComun','usuarioComun','jose@hotmail.com','1234567',0,1);
--USUARIOS

--PERMISOS
INSERT INTO personaPermisos(rut,cargo,descripcion)values(17707464,1,'administrador,administrador,administra todo el sitio, el administrador se encuentra en casa central skberge');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(11111111,2,'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(22222222,3,'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(33333333,4,'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio');
--PERMISOS

--MARCAS
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('SSANGYONG','SSY','marca de auto','BC08');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('MITSUBISHI','MMC','marca de auto','BC04');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('FIAT','IT','marca de auto','BC03');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('CHRYSLER','CH','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('CHERY','NW','marca de auto','BC10');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('MG','MG','marca de auto','BC11');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('TATA','TATA','marca de auto','BC13');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('JEEP','JP','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('DODGE','DG','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('ALFA ROMEO','AR','marca de auto','BC03');
--MARCAS

--MARCAS SEGUN CONCESIONARIO

--SSANGYONG
INSERT INTO concesionarioMarca VALUES('SSANGYONG','AUTOSUMMIT');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','CARTONI EXPRESS');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','AUTOMOTRIZ VAL LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','CARTONI');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','LE BLANC LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','COMERCIAL EXPOAUTOS');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','GMB');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','AVENTURA MOTORS');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','MARCELO FRONZA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','MARCO RATTI');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','GOMA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','AUTOMOTRIZ VARONA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','QUILIN');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','NALLAR');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','COMERCO');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','SIGLO XXI');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','COMERCIAL NOACK LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','APC COMERCIAL LTDA');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','TRANSWORLD');
--SSANGYONG

--MITSUBISHI
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','AUTOSUMMIT');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','CARTONI EXPRESS');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','AUTOMOTRIZ VAL LTDA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','JOSE ETEROVIC');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','GMB');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','SERVITAL');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','MARCO RATTI');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','AVENTURA MOTORS');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','MARCELO FRONZA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','LA FORESTA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','PATRICIO GARCIA Y CIA LTDA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','AUTOIMPACTO');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','AUTOMOTRIZ VARONA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','DIFOR CHILE SA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','JORGE OSORIO');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','KORNER');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','NALLAR');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','PINTURAS AUTOMOTRICES');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','COMERCO');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','COMERCIAL NOACK LTDA');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','SIGLO XXI');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','TRANSWORLD');
--MITSUBISHI

--FIAT
INSERT INTO concesionarioMarca VALUES('FIAT','AUTOSUMMIT');
INSERT INTO concesionarioMarca VALUES('FIAT','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('FIAT','CARTONI EXPRESS');
INSERT INTO concesionarioMarca VALUES('FIAT','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('FIAT','CARTONI');
INSERT INTO concesionarioMarca VALUES('FIAT','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('FIAT','GMB');
INSERT INTO concesionarioMarca VALUES('FIAT','PIAMONTE SA');
INSERT INTO concesionarioMarca VALUES('FIAT','AVENTURA MOTORS');
INSERT INTO concesionarioMarca VALUES('FIAT','VEGA ARTUS');
INSERT INTO concesionarioMarca VALUES('FIAT','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('FIAT','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('FIAT','MARCELO FRONZA');
INSERT INTO concesionarioMarca VALUES('FIAT','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('FIAT','APC COMERCIAL LTDA');
INSERT INTO concesionarioMarca VALUES('FIAT','GOMA');
INSERT INTO concesionarioMarca VALUES('FIAT','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('FIAT','AUTOMOTRIZ VARONA');
INSERT INTO concesionarioMarca VALUES('FIAT','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('FIAT','QUILIN');
INSERT INTO concesionarioMarca VALUES('FIAT','NALLAR');
INSERT INTO concesionarioMarca VALUES('FIAT','BAUER');
INSERT INTO concesionarioMarca VALUES('FIAT','SIGLO XXI');
INSERT INTO concesionarioMarca VALUES('FIAT','COMERCO');
INSERT INTO concesionarioMarca VALUES('FIAT','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('FIAT','TRANSWORLD');
--FIAT

--CHRYSLER 
INSERT INTO concesionarioMarca VALUES('CHRYSLER','RODAR');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','CARTONI EXPRESS');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','AUTOMOTRIZ VAL LTDA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','CARTONI');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','HERNANDEZ MOTORES');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','LE BLANC LTDA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','COMERCIAL EXPOAUTOS');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','JOSE VERGARA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','GMB');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','VEGA ARTUS');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','AVENTURA MOTORS');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','FELIPE NOGUERA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','GOMA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','AUTOFRANCE');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','AUTOMOTRIZ VARONA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','NALLAR');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','SIGLO XXI');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','COMERCO');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','TRANSWORLD');
--CHRYSLER 

--CHERY 
INSERT INTO concesionarioMarca VALUES('CHERY','AUTOSUMMIT');
INSERT INTO concesionarioMarca VALUES('CHERY','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('CHERY','CARTONI EXPRESS');
INSERT INTO concesionarioMarca VALUES('CHERY','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('CHERY','HERNANDEZ MOTORES');
INSERT INTO concesionarioMarca VALUES('CHERY','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('CHERY','GMB');
INSERT INTO concesionarioMarca VALUES('CHERY','PIAMONTE SA');
INSERT INTO concesionarioMarca VALUES('CHERY','SERVITAL');
INSERT INTO concesionarioMarca VALUES('CHERY','MARCO RATTI');
INSERT INTO concesionarioMarca VALUES('CHERY','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('CHERY','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('CHERY','MARCELO FRONZA');
INSERT INTO concesionarioMarca VALUES('CHERY','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('CHERY','GOMA');
INSERT INTO concesionarioMarca VALUES('CHERY','AUTOFRANCE');
INSERT INTO concesionarioMarca VALUES('CHERY','APC COMERCIAL LTDA');
INSERT INTO concesionarioMarca VALUES('CHERY','BRUNO FRITSCH');
INSERT INTO concesionarioMarca VALUES('CHERY','AUTOMOTRIZ VARONA');
INSERT INTO concesionarioMarca VALUES('CHERY','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('CHERY','NALLAR');
INSERT INTO concesionarioMarca VALUES('CHERY','SIGLO XXI');
INSERT INTO concesionarioMarca VALUES('CHERY','COMERCO');
INSERT INTO concesionarioMarca VALUES('CHERY','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('CHERY','TRANSWORLD');
--CHERY 

--MG 
INSERT INTO concesionarioMarca VALUES('MG','AUTOSUMMIT');
INSERT INTO concesionarioMarca VALUES('MG','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('MG','CARTONI');
INSERT INTO concesionarioMarca VALUES('MG','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('MG','COMERCIAL AUTOMOTRIZ SA PALD');
INSERT INTO concesionarioMarca VALUES('MG','GMB');
INSERT INTO concesionarioMarca VALUES('MG','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('MG','MARCO RATTI');
INSERT INTO concesionarioMarca VALUES('MG','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('MG','GOMA');
INSERT INTO concesionarioMarca VALUES('MG','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('MG','TRANSWORLD');
INSERT INTO concesionarioMarca VALUES('MG','NOVICIADO');
--MG 

--TATA 
INSERT INTO concesionarioMarca VALUES('TATA','SK COMERCIAL');
INSERT INTO concesionarioMarca VALUES('TATA','CALLEGARI LTDA');
INSERT INTO concesionarioMarca VALUES('TATA','CARTONI');
INSERT INTO concesionarioMarca VALUES('TATA','REPUESTOS EXPRESS');
INSERT INTO concesionarioMarca VALUES('TATA','RODAR');
INSERT INTO concesionarioMarca VALUES('TATA','PIAMONTE SA');
INSERT INTO concesionarioMarca VALUES('TATA','RENTAL SA');
INSERT INTO concesionarioMarca VALUES('TATA','ROSSELOT LTDA');
INSERT INTO concesionarioMarca VALUES('TATA','VEGA ARTUS');
INSERT INTO concesionarioMarca VALUES('TATA','APC COMERCIAL LTDA');
INSERT INTO concesionarioMarca VALUES('TATA','BAUER');
INSERT INTO concesionarioMarca VALUES('TATA','COMERCO');
INSERT INTO concesionarioMarca VALUES('TATA','CORDILLERA');
INSERT INTO concesionarioMarca VALUES('TATA','TRANSWORLD');
INSERT INTO concesionarioMarca VALUES('TATA','GOMA');
INSERT INTO concesionarioMarca VALUES('TATA','NALLAR');
INSERT INTO concesionarioMarca VALUES('TATA','PATAGONIA');
INSERT INTO concesionarioMarca VALUES('TATA','NOVICIADO');
INSERT INTO concesionarioMarca VALUES('TATA','AUTOMOTRIZ VARONA');
--TATA 


--SKBERGE CASA MATRIZ (TODAS LAS MARCAS)
INSERT INTO concesionarioMarca VALUES('ALFA ROMEO','SKBERGE');
INSERT INTO concesionarioMarca VALUES('CHERY','SKBERGE');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','SKBERGE');
INSERT INTO concesionarioMarca VALUES('DODGE','SKBERGE');
INSERT INTO concesionarioMarca VALUES('FIAT','SKBERGE');
INSERT INTO concesionarioMarca VALUES('JEEP','SKBERGE');
INSERT INTO concesionarioMarca VALUES('MG','SKBERGE');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','SKBERGE');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','SKBERGE');
INSERT INTO concesionarioMarca VALUES('TATA','SKBERGE');
--SKBERGE CASA MATRIZ (TODAS LAS MARCAS)

--COTIZADORES POSEE TODAS LAS MARCAS
INSERT INTO concesionarioMarca VALUES('ALFA ROMEO','cotizadores');
INSERT INTO concesionarioMarca VALUES('CHERY','cotizadores');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','cotizadores');
INSERT INTO concesionarioMarca VALUES('DODGE','cotizadores');
INSERT INTO concesionarioMarca VALUES('FIAT','cotizadores');
INSERT INTO concesionarioMarca VALUES('JEEP','cotizadores');
INSERT INTO concesionarioMarca VALUES('MG','cotizadores');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','cotizadores');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','cotizadores');
INSERT INTO concesionarioMarca VALUES('TATA','cotizadores');
--COTIZADORES POSEE TODAS LAS MARCAS

--MODELO
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Wrangler','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Patriot ','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Compass','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Cherokee','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Grand Cherokee','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHRYSLER','Town & Country','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Durango','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Journey','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Nitro','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Caliber','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Dacota','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','RAM','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Challenger','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('TATA','Xenon','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','500','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Bravo','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Grande Punto','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Grande Punto Diesel','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Punto EVO','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Linea','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Palio','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Strada','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Strada Doble Cabina','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Doblo','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Ducato','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Fiorino City','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Fiorino','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','I-Miev','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Lancer Evolution','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Lancer','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Lancer Serie R','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','ASX','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Outlander K2','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Montero Sport G2','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Montero Sport 2.5','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Montero','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','L200 Work','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','L200 CR Katana','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','L200 Katana CR','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','L200 Katana','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','L200 Dakar','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Actyon Sport','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Rexton','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Korando','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Kyron','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Actyon ','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Stavic','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','IQ','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Face','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Skin','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Tiggo','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Destiny','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Beat','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Giulietta','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','159','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Brera','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Mito','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','350','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','550','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','750','Modelo de la marca');
	INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','MG-6','Modelo de la marca');
--MODELO

--MARCAS SEGUN CONCESIONARIO
/*

select*from personaPermisos
select*from persona
select*from zona
select*from concesionario
select*from marca
select*from concesionarioMarca
select*from sucursal

--RELACIONES
--PERSONA/CONCESIONARIO/CARGO
select persona.nombre,sucursal.nombreSucursal as DealerEmpresa,concesionario.tipo,sucursal.shipCode,sucursal.regionSucursal,sucursal.provinciaSucursal,personaPermisos.cargo 
from personaPermisos,persona,concesionario,sucursal
where personaPermisos.rut = persona.rut 
and sucursal.nombreConcesionario = concesionario.nombreConcesionario
and sucursal.shipCode = persona.shipCode

--CONCESIONARIOS/ZONAS
select concesionario.nombreConcesionario,concesionario.tipo,zona.sector
from concesionario, zona
where zona.sector='norte' and zona.zona = concesionario.zona


select concesionario.nombreConcesionario,concesionario.tipo,zona.sector
from concesionario, zona
where zona.sector='centro' and zona.zona = concesionario.zona


select concesionario.nombreConcesionario,concesionario.tipo,zona.sector
from concesionario, zona
where zona.sector='sur' and zona.zona = concesionario.zona

--MARCAS DE CONCESIONARIO POR ZONA
select concesionario.nombreConcesionario,marca.nombreMarca,zona.sector
from concesionario, zona,concesionarioMarca,marca
where zona.sector='sur' and zona.zona = concesionario.zona
and concesionario.nombreConcesionario = concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca = marca.nombreMarca
group by concesionario.nombreConcesionario,marca.nombreMarca,zona.sector
--MARCAS DE CONCESIONARIO POR ZONA

--MARCA/CONCESIONARIO
--SSANGYONG
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='SSANGYONG'

--MITSUBISHI
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='MITSUBISHI'

--FIAT
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='FIAT'

--CHRYSLER
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='CHRYSLER'

--CHERY
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='CHERY'

--MG
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='MG'

--TATA
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and marca.nombreMarca='TATA'

--CONCESIONARIO/MARCA
select concesionario.nombreConcesionario,marca.nombreMarca
from concesionario,marca,concesionarioMarca
where concesionario.nombreConcesionario =concesionarioMarca.nombreConcesionario
and concesionarioMarca.nombreMarca =marca.nombreMarca
and concesionario.nombreConcesionario ='RODAR'

RODAR, REPUESTOS EXPRESS, CARTONI EXPRESS, CALLEGARI LTDA, AUTOMOTRIZ VAL LTDA, CARTONI, HERNANDEZ MOTORES 
LE-BLANC LTDA, COMERCIAL EXPOAUTOS, JOSE VERGARA, SK COMERCIAL, AUTOSUMMIT, JOSE ETEROVIC, G.M.B., VEGA ARTUS, AVENTURA MOTORS
ROSSELOT LTDA, RENTAL S.A., PALD, FELIPE NOGUERA, MARCELO FRONZA, MARCO RATTI, SERVITAL, LA FORESTA
PATRICIO GRACIA, PIAMONTE S.A., GOMA, AUTOFRANCE, CORDILLERA, AUTOMOTRIZ VARONA, NALLAR, PATAGONIA, SIGLO XXI
COMERCO, NOVICIADO, TRANSWORLD, QUILIN, NOACK LTDA, APC, AUTOIMPACTO, DIFOR CHILE S.A., JORGE OSORIO, KORNER
PINTURAS AUTOMOTRICES, BAUER, BRUNO FRITSCH, MARCO RATTI SUR

--TRIMESTRES

select sum(compras) as primerTrimestre from  indicador where mes<4

select sum(compras) as segundoTrimestre from  indicador where mes between 4 and 6

select sum(compras) as tercerTrimestre from  indicador where mes between 7 and 9

select sum(compras) as cuartoTrimestre from  indicador where mes between 10 and 12
--TRIMESTRES

-- MARCA PARA LOS CONCESIONARIOS SEGUN MARCA DE EXCEL EN CONCESIONARIOMARCA
select nombreMarca from concesionarioMarca where idConcesionarioMarca between 185 and 212 group by nombreMarca
-- MARCA PARA LOS CONCESIONARIOS SEGUN MARCA DE EXCEL EN CONCESIONARIOMARCA
*/





/*
INDICADOR 

--INDICADOR POR MARCA
SELECT concesionarioMarca.nombreMarca,concesionarioMarca.nombreConcesionario,indicador.ano,indicador.mes,indicador.compras,indicador.metas
FROM  concesionarioMarca,indicador 
WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
AND concesionarioMarca.nombreMarca=LTRIM('CHRYSLER')

--INDICADOR POR CONCESIONARIO
SELECT concesionarioMarca.nombreMarca,concesionarioMarca.nombreConcesionario,indicador.ano,indicador.mes,indicador.compras,indicador.metas
FROM  concesionarioMarca,indicador 
WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
AND concesionarioMarca.nombreConcesionario=LTRIM('RODAR')

--INDICADOR SEGUN MARCA Y SU CORRESPONDIENTE CONCESIONARIO
SELECT concesionarioMarca.nombreMarca,concesionarioMarca.nombreConcesionario,indicador.ano,indicador.mes,indicador.compras,indicador.metas
FROM  concesionarioMarca,indicador 
WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
AND concesionarioMarca.nombreMarca=LTRIM('CHRYSLER')
AND concesionarioMarca.nombreConcesionario=LTRIM('RODAR')


--COMPRAS POR TRIMESTRE SEGUN MARCA Y CONCESIONARIO
SELECT SUM(indicador.compras)
FROM  concesionarioMarca,indicador 
WHERE concesionarioMarca.idConcesionarioMarca = indicador.idConcesionarioMarca 
AND concesionarioMarca.nombreMarca=LTRIM('CHRYSLER')
AND concesionarioMarca.nombreConcesionario=LTRIM('REPUESTOS EXPRESS')
AND indicador.mes<4

USE MASTER
GO
DROP DATABASE skberge

*/

