CREATE DATABASE skberge
GO
USE skberge
GO 

--PROCEDIMIENTOS DEL USUARIO(PERSONA)
	CREATE PROCEDURE sp_loguin @usuario VARCHAR(50), @contrasena VARCHAR(20)
	AS
	BEGIN
	SELECT persona.nombre,personaPermisos.cargo,persona.contrasena, SUBSTRING(nombre,0,5)+SUBSTRING(CAST(persona.rut as VARCHAR),5,4),persona.rut
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
		  DECLARE @largo  int;
		  SET @largo =LEN(@rut)-3
	  
		  IF @cargo=1 BEGIN SET @descripcion =  'administrador,administrador,administra todo el sitio, el administrador se encuentra en casa central skberge' SET @codSucursal='X' END
		  ELSE IF @cargo=2 BEGIN SET @descripcion =  'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO' SET @codSucursal=@shipCode END
		  ELSE IF @cargo=3 BEGIN SET @descripcion =  'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios' SET @codSucursal=@shipCode END 
		  ELSE IF @cargo=4 BEGIN SET @descripcion =  'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio' SET @nombreConcesionario='cotizadores' SET @nombreEmpresa=@empresa SET @codSucursal=@shipCode END
		
		  IF EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado)VALUES(@codSucursal,@rut,@nombre,@rut,ENCRYPTBYPASSPHRASE('ENCRIPTADO',SUBSTRING(@nombre,0,5)+SUBSTRING(CAST(@rut as VARCHAR),@largo,4)),@email,@telefono,@multipleSucursal,1); INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,@cargo,@descripcion)	 
		  END
		  ELSE IF NOT EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,shipCode,direccionSucursal)VALUES(@nombreConcesionario,'Santiago','',@codSucursal,@codSucursal); INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado)VALUES(@codSucursal,@rut,@nombre,@rut,ENCRYPTBYPASSPHRASE('ENCRIPTADO',SUBSTRING(@nombre,0,5)+SUBSTRING(CAST(@rut as VARCHAR),@largo,4)),@email,@telefono,@multipleSucursal,1); INSERT INTO personaPermisos(rut,cargo,descripcion)VALUES(@rut,@cargo,@descripcion);
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

	--MODIFICA PERSONA
	CREATE PROCEDURE sp_modificaPersona(@empresa VARCHAR(50),@shipCode VARCHAR(50),@rut INTEGER,@nombre VARCHAR(50),@email VARCHAR(50),@telefono VARCHAR(50),@cargo INTEGER,@multipleSucursal BIT)
	AS
	BEGIN
	BEGIN TRY
		BEGIN TRAN
	   
		  DECLARE @descripcion VARCHAR(255);
		  DECLARE @nombreConcesionario VARCHAR(255);
		  DECLARE @nombreEmpresa VARCHAR(255);
		  DECLARE @codSucursal VARCHAR(25);  
		  DECLARE @largo  int;
		  SET @largo =LEN(@rut)-3
	  
		  IF @cargo=1 BEGIN SET @descripcion =  'administrador,administrador,administra todo el sitio, el administrador se encuentra en casa central skberge' SET @codSucursal='X' END
		  ELSE IF @cargo=2 BEGIN SET @descripcion =  'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO' SET @codSucursal=@shipCode END
		  ELSE IF @cargo=3 BEGIN SET @descripcion =  'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios' SET @codSucursal=@shipCode END 
		  ELSE IF @cargo=4 BEGIN SET @descripcion =  'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio' SET @nombreConcesionario='cotizadores' SET @nombreEmpresa=@empresa SET @codSucursal=@shipCode END
		
		  IF EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			UPDATE persona SET shipCode=@codSucursal,nombre=@nombre,email=@email,telefono=@telefono,multipleSucursal=@multipleSucursal WHERE rut=@rut; UPDATE personaPermisos SET cargo=@cargo,descripcion=@descripcion WHERE rut=@rut;	 
		  END
		  ELSE IF NOT EXISTS (select shipCode from sucursal where shipCode=@codSucursal)
		  BEGIN
			INSERT INTO sucursal(nombreConcesionario,nombreComuna,nombreSucursal,organizacionVenta,numeroFactura,rutHolding,shipCode,direccionSucursal)VALUES(@nombreConcesionario,'Santiago',@nombreEmpresa,'','','',@codSucursal,@codSucursal); 	UPDATE persona SET shipCode=@codSucursal,nombre=@nombre,email=@email,telefono=@telefono,multipleSucursal=@multipleSucursal WHERE rut=@rut; UPDATE personaPermisos SET cargo=@cargo,descripcion=@descripcion WHERE rut=@rut;	 
		
		  END
			COMMIT TRAN
		END TRY
		BEGIN CATCH
		   ROLLBACK TRAN
		   RAISERROR('se ha producido un error,incosistencia de datos o Puede que se esten duplicando datos', 16, 1);
		END CATCH
	END --FIN STOREPROCEDURE sp_modificaPersona
	GO

	CREATE PROCEDURE sp_modificaContrasena(@rut INT,@nuevaContrasena VARCHAR(50))
	AS
	BEGIN
	UPDATE persona  SET contrasena=ENCRYPTBYPASSPHRASE('ENCRIPTADO',@nuevaContrasena) WHERE rut=@rut;
	END
	GO
	--EL SIGUIENTE SP RESETEA LA CONTRASENA DE FABRICA PARA EL USUARIO SEGUN SU RUT INDICADO
	CREATE PROCEDURE sp_reseteaContrasena(@rut INT)
	AS
	BEGIN
	DECLARE @largo  int;
	SET @largo =LEN(@rut)-3
	UPDATE persona  SET contrasena=ENCRYPTBYPASSPHRASE('ENCRIPTADO',((SELECT SUBSTRING(nombre,0,5) FROM persona WHERE rut=@rut)+SUBSTRING(CAST(@rut as VARCHAR),@largo,4))) WHERE rut=@rut;
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
				INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES(@sector,@nombre,'consecionario',@imagen, @rutHolding, @numeroFactura)
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			RAISERROR('Error en el procedimiento almacenado',1,1)
		END CATCH
	END --FIN DEL PROCEDIMIENTO
	GO
	
	CREATE PROCEDURE sp_insertaSucursal(@nombreConcesionario VARCHAR(50),@nombreComuna VARCHAR(50),@numeroFactura VARCHAR(50),@shipCode VARCHAR(50),@direccionSucursal VARCHAR(50))
	AS
	BEGIN
		BEGIN TRY
			BEGIN TRAN
				INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,shipCode,direccionSucursal)VALUES(@nombreConcesionario,@nombreComuna,@numeroFactura,@shipCode,@direccionSucursal)
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
  nombreModelo VARCHAR(50) NOT NULL /*UNIQUE*/ ,
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
  numeroFactura VARCHAR(50) NOT NULL,
  shipCode VARCHAR(50) NOT NULL UNIQUE,
  direccionSucursal VARCHAR(100) NOT NULL ,
  PRIMARY KEY(idSucursal),
  FOREIGN KEY(nombreConcesionario)
  REFERENCES concesionario(nombreConcesionario)
  ON DELETE CASCADE
  ON UPDATE CASCADE
  );


CREATE TABLE persona (
  idPersona INTEGER NOT NULL IDENTITY(1000,1),
  shipCode VARCHAR(50) NOT NULL,
  rut BIGINT NOT NULL UNIQUE,
  nombre VARCHAR(50) NOT NULL,
  usuario VARCHAR(50) NOT NULL,
  contrasena VARBINARY(8000) /*NOT NULL*/,
  email VARCHAR(50) NOT NULL,
  telefono VARCHAR(50) NOT NULL,
  multipleSucursal BIT NOT NULL,
  habilitado BIT NOT NULL,
  PRIMARY KEY(idPersona),
  /*FOREIGN KEY(shipCode)
    REFERENCES sucursal(shipCode)
      ON DELETE CASCADE
      ON UPDATE CASCADE*/
);

create table contenido_persona(
	id INTEGER NOT NULL IDENTITY(10,1),
	idContenido INTEGER NOT NULL,
		FOREIGN KEY(idContenido) 
		REFERENCES contenido(idContenido)
		ON DELETE CASCADE
		ON UPDATE CASCADE,
	rut BIGINT NOT NULL
		FOREIGN KEY(rut) 
		REFERENCES persona(rut)
		ON DELETE CASCADE
		ON UPDATE CASCADE
);

CREATE TABLE personaPermisos (
  idPermisos INTEGER  NOT NULL IDENTITY(1,1),
  rut BIGINT NOT NULL,
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

CREATE TABLE boletines_persona(
	id INTEGER NOT NULL IDENTITY(10,1),
	idBoletin INTEGER  NOT NULL,
		foreign key(idBoletin)
		references boletines(idBoletin)
		on delete cascade
		on update cascade,
	rutUser BIGINT NOT NULL
		foreign key(rutUser)
		references persona(rut)
		on delete cascade
		on update cascade
);

CREATE TABLE indicador(
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
	--termino datetime not null,
	PRIMARY KEY(idEncuesta)
);

create table encuesta_user(
	idEncuesta integer not null,
	rutUser varchar(10) not null
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
		id_pedido varchar(500) not null,
		rut_user varchar(10) not null,
		fecha_creacion datetime not null,
		codigo_rep varchar(50) not null,
		marca varchar(50) not null,
		cantidad int not null,
		detalle_rep varchar(200) not null		
);
--select * from BACKORDER
--delete from BACKORDER
--DROP TABLE DESCARTADOS
--//////////////////////////////////////////////////////////


--TABLAS DESCARTADOS/////////////////////////////////////////
CREATE TABLE DESCARTADOS(
		idDescartado int identity(100,1) primary key,
		id_pedido varchar(500) not null,
		rut_user varchar(10) not null,
		fechaDescarte datetime not null,
		codigo_rep varchar(50) not null,
		marca varchar(50) not null,
		cantidad int not null,
		detalle_rep varchar(200) not null				
		);

--TABLA VFC /////////////////////////////////////////
CREATE TABLE VFC(
		num_VFC varchar(500) primary key not null,
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

create table no_stock(
	idSession varchar(100) not null,
	vfc varchar(100) not null,
	reserva char(1) not null,
	descarte char(1) not null,
	marca varchar(50) not null, grupoMat varchar(5) not null,
	codigo varchar(50) not null,
	descripcion varchar(100) not null,
	cantidad int not null, stock int not null,
	precioC int not null,
	totalC int not null,
	precioL int not null,
	totalL int not null
);

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
	[FECHA_EXPIRACION_SYS] [datetime] NULL,
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
		declare @fecha_ext int=0;
		set @fecha=(select cast(FECHA_SOLICITUD as int)+4 from pedido where id_pedido = @id_pedido);
		set @fecha_ext=(select cast(FECHA_SOLICITUD as int)+14 from pedido where id_pedido = @id_pedido);
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
	[PREFIJO] [varchar](10) NULL,
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

--Pedido Preferido---------------------------------------------------------select * from carro
CREATE TABLE PREFERIDO(
			idPreferido[INTEGER] IDENTITY(100,1) PRIMARY KEY NOT NULL,
			idSession varchar(100) not null,
			rutCreador varchar(10) not null,
			fechaCreacion datetime not null,
		    textoDescrip varchar(100) not null
			);
CREATE TABLE PREFERIDO_REPUESTO(
		    idPreferido varchar(100) not null,
			marca varchar(50) not null, grupoMat varchar(5) not null,
			codigo varchar(50) not null,
			descripcion varchar(100) not null,
			cantidad int not null, stock int not null,
			precioC int not null,
			totalC int not null,
			precioL int not null,
			totalL int not null
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

--CONCESIONARIOS
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','APC Servicios','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76433870-7','232097');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Autofrance','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','84807200-1','232661');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Autoimpacto Chillan','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78453310-7','232183');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Automotora Goma','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','79690930-7','232234');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Automotriz Fernando Korner','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78510990-2','259570');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Automotriz Val','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','87831800-5','232326');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Autosummit','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96924460-8 ','234111');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Autosummit Stgo','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96924460-8 ','234111');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Aventura Motors ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76186070-4','232095');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Bruno Fritsch','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','84807200-1','232661');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Callegari e Hijos La Serena','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','84916800-2','232662');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Cartoni','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','85430500-K','232312');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Comasa Casa Matriz','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','COMASA PALD','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Comasa Santa Isabel','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Comasa Vitacura','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Comercial Automotora Prime','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','77456570-1','232125');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Comercial Rosselot','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96502140-K','232416');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Difor','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96918300-5','232523');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Ditalcar','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76939200-9','237481');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Emprenani','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78795760-9','232627');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Expoautos','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78027430-1','232159');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Felipe Noguera ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78711940-9','235429');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Hernandez Motores ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','94859000-K','232695');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Jaime Navarrete Garcia','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','14264517-3','201145');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Jorge Osorio Uribe','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','5362093-0','217236');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Jose Eterovic','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','2972644-2','2001498');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Jose Vergara','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','81365800-3','232096');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','La Foresta ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78232780-1','232170');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Marcelo Fronza y Cia Ltda','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78098600-K','232164');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Marco Ratti ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','4942962-2','200094');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Nallar Autos Osorno','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','79834820-5','232246');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Noack','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76253220-4','217429');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Patagonia Automotriz','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','99575100-3','232754');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Patricio Gracia ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','79897420-3','202918');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Piamonte','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96642160-6','232457');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Pinturas Automotrices P Montt','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78516790-2','234402');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Rental ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78276630-9','232174');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Repuestos Express','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96928530-4','IBC05');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Repuestos Express Viña','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76105295-5','263195');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Rodar','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','79609330-7','232226');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','SKCOMERCIAL SA','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','84196300-8','ICC01');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Servicio Automotrices Integrales Ltda','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76031581-8','266667');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Servicio Tecnico GMB','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96564810-0','200088');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Servicios Cordillera','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','79853470-K','237622');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Servital','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','96812030-1','232730');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Siglo XXI','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','78770630-4','232199');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','SKBerge Logistica SA','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76040171-4','IBC15');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Norte','Sociedad Hermanas Callegari','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76349970-7','200100');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Transworld Punta Arenas','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','77781260-2','232627');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Sur','Varona Coyhaique','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','76101820-5','232093');
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura)VALUES('Centro','Vega Artus ','concesionario','../doc/imgAccesorios/imgConcesionariosjpg','77810800-5','201016');


--CASA CENTRAL
INSERT INTO concesionario(sector,tipo,nombreConcesionario,imagen,rutHolding,numeroFactura)VALUES('centro','casa central','SKBERGE','../doc/imgConcesionarios/imgConcesionarios.jpg','X','X');
--CASA CENTRAL

--COTIZADORES
INSERT INTO concesionario(sector,nombreConcesionario,tipo,imagen,rutHolding,numeroFactura) values('centro','cotizadores','cotizadores','../doc/imgAccesorios/imgConcesionarios.jpg','','');
--COTIZADORES

--SUCURSALES
-- Abriendo: sucursal.csv.
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('APC Servicios','','232097','232097','Av Vicuña Mackenna 1341 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autofrance','','232661','232661','Arturo Prat 320 Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autofrance','','232661','3100000970','OHIGGINS 132 Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autofrance','','232661','3100000971','Jorge Alessandri 3177 Talcahuano Local D-101');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autoimpacto Chillan','','232183','3100000917','Claudio Arrau 1043 Chillan');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Automotora Goma','','232234','3100001045','General Mackenna 1030 Temuco');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Automotriz Fernando Korner','','259570','259570','Angol 920 Concepción');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Automotriz Val','','232326','232326','Paula Jaraquemada  1231 Arica');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autosummit','','234111','3100001007','Onix 85 Antofagasta');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autosummit','','234111','3100001009','Granaderos 3417 Calama');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autosummit Stgo','','234111','3100001005','Pedro Aguirre Cerda 5613 PAC Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Autosummit Stgo','','234111','234111','Av Vicuña Mackenna 5495 San Joaquin');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Aventura Motors ','','232095','3100001048','Santa Elena de Huechuraba 1135 Huachuraba');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Aventura Motors ','','232095','3100000967','Vitacura 7408 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Aventura Motors ','','232095','3100001090','Tabancura 1775 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Aventura Motors ','','232095','3100001420','El Salto 3810 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Aventura Motors ','','232095','3100001472','Avda Manquehue Sur 652 Las Condes');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Bruno Fritsch','','232661','3100001004','Vespucio Sur 1501 Local 108 al 142 Mall Plaza Oeste Cerrillos');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Bruno Fritsch','','232661','3100001002','Camino a Melipilla 9160 Cerrillos Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Bruno Fritsch','','232661','3100001080','10 de Julio 342 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Bruno Fritsch','','232661','3100001006','Av Vicuña Mackenna 7110 Local 104 y 105 Mall Plaza Vespucio Autoplaza La Florida');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Callegari e Hijos La Serena','','232662','232662','Av El Salto 1695 La Serena');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Callegari e Hijos La Serena','','232662','3100000916','Balmaceda 1880 La Serena');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Cartoni','','232312','3100000944','Calle Limache 4299 Valparaiso');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Cartoni','','232312','3100000930','ARLEGUI 145 Viña Del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Cartoni','','232312','3100001087','Chacabuco 2012 Valparaiso');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comasa Casa Matriz','','IBC05','IBC05','Americo Vespucio Norte 1601 Quilicura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('COMASA PALD','','IBC05','3100000943','Av La Dehesa 1993');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comasa Santa Isabel','','IBC05','3100000960','Santa Isabel 360 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comasa Santa Isabel','','IBC05','3100000964','Mall Plaza Sur Autoplaza San Bernardo');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comasa Santa Isabel','','IBC05','3100000965','Americo Vespucio Norte 1155 Movicenter');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comasa Vitacura','','IBC05','3100000952','Vitacura 8191 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Automotora Prime','','232125','3100000906','Av Granaderos  3120 Calama');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Automotora Prime','','232125','3100000908','Av Granaderos  2625 Calama');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Automotora Prime','','232125','3100001085','Condell 2707 Antofagasta');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Automotora Prime','','232125','232125','Perez Zujovic 4534 Mall Plaza Antofagasta');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','232338','Av Vicuña Mackenna 1911 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100000994','Av Colon 6692 Las Condes');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100000934','8 Norte 837 Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100000957','Panamericana Sur Km 186 Curico');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100000961','Av San Miguel cruce Varoli 2710 Talca');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100000920','Av 21 Mayo 281 Quillota');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100001093','Francisco Bilbao 2139 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','232416','Limache 3865 El Salto Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Comercial Rosselot','','232416','3100001047','Quillota 826  Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Difor','','232523','3100001030','Longitudinal Sur Km 509 Los Angeles');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Difor','','232523','3100000968','Camino a Coronel 3455 San Pedro De La Paz Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Difor','','232523','3100000907','San Martin 886 Temuco');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Difor','','232523','3100000935','Panamericana 161 Puerto Montt');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Ditalcar','','237481','237481','Av Las Condes 8127 Las Condes');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Ditalcar','','237481','3100001000','Av Camilo Henriquez 3692 Mall Plaza Tobalaba Loc 140 Puente Alto');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Emprenani','','232627','232627','Errazuriz 853 Piso 3 Punta Arenas');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Emprenani','','232627','3100001018','Av Principal Sitio 43-44 Punta Arenas');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Emprenani','','232627','232147','Av Bulnes 03545 Punta Arenas');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Expoautos','','232159','232159','Luis Pasteur 6705 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Expoautos','','232159','3100000976','Av Las Torres 1432 Huechuraba');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Felipe Noguera ','','235429','235429','Av Kennedy 7286 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Hernandez Motores ','','232695','3100000948','Chacabuco 2012 Valparaiso');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Hernandez Motores ','','232695','232695','15 Norte 1018 Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Hernandez Motores ','','232695','3100000938','3 Oriente 1374 Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Hernandez Motores ','','232695','3100000936','3 1/2 Oriente 1240 Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Jaime Navarrete Garcia','','201145','201145','1SUR 14 Y 15 ORIENTE 2158A Talca');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Jorge Osorio Uribe','','217236','217236','Pedro Aguirre Cerda 28 Chillan');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Jose Eterovic','','2001498','200198','Tocornal 655 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Jose Vergara','','232096','234265','ROMAN DIAZ 1263 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('La Foresta ','','232170','235425','AvPadre Hurtado 1382 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('La Foresta ','','232170','3100001029','Vicuña Mackenna 49 Melipilla Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('La Foresta ','','232170','3100001027','Av Vitacura 5590 Vitacura');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Marcelo Fronza y Cia Ltda','','232164','232164','HONTANEDA 2615 Valparaiso');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Marcelo Fronza y Cia Ltda','','232164','3100000922','21 De Mayo 570 Quillota');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Marcelo Fronza y Cia Ltda','','232164','3100001088','Victoria 3033 Valparaiso');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Marco Ratti ','','200094','200094','AvLib B OHiggins 214 Rancagua');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Marco Ratti ','','200094','3100000947','AvLib B OHiggins  216 Rancagua');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Nallar Autos Osorno','','232246','232246','Av Juan Mackenna 1702 Osorno');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Noack','','217429','3100001013','El Aguilucho 3398 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Noack','','217429','200060','Lo Herrero 8828 Parque industrial La Reina');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Noack','','217429','232096','El Aguilucho 3380 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Patagonia Automotriz','','232754','232754','Benavente 690 Puerto Montt');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Patagonia Automotriz','','232754','3100001043','Calle Pilpilco 530Parque Industrial Puerto Montt');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Patagonia Automotriz','','232754','3100000933','Bio-Bio 1150 Puerto Varas');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Patricio Gracia ','','202918','234070','Luis Beltran 2219 Ñuñoa');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Patricio Gracia ','','202918','3100001025','Av Las Condes 8506 Las Condes');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Piamonte','','232457','232457','Irarrazaval 3400');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Piamonte','','232457','3100001008','Agustinas 2138 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Piamonte','','232457','3100001046','Gran Avenida 7759 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Pinturas Automotrices P Montt','','234402','234402','Pasaje San Andres 60 Cardonal Puerto Montt');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rental ','','232174','232174','Bernardo OHiggins 776 San Fernando');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rental ','','232174','3100000951','Bernardo OHiggins 379 San Fernando');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Repuestos Express','','IBC05','3100000954','Santa Rosa 812 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Repuestos Express','','IBC05','3100000956','10 de Julio 727 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Repuestos Express','','IBC05','3100001506','Gran Avenida 6798 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Repuestos Express','','IBC05','3100000958','Brasil 26 Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Repuestos Express Viña','','263195','263195','San Antonio 1176 Viña del Mar');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rodar','','232226','3100000905','Tacna 55 Antofagasta');
/*INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rodar','','232226','232226','Av Granaderos  3120 Calama');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rodar','','232226','3100000910','Av Argentina 3137 Calama');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rodar','','232226','3100000904','Perez Zujovic 4534 Mall Plaza Antofagasta');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Rodar','','232226','3100000908','Av Granaderos  2625 Calama');*/
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('SKCOMERCIAL SA','','ICC01','3100000903','Santa Rosa de Huara Sitio 19 Manzana C Barrio Industrial Zofri Iquique');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('SKCOMERCIAL SA','','ICC01','ICC01','Panamericana Norte Km 15 1/2 Lampa Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Automotrices Integrales Ltda','','266667','266667','AvPedro Aguiire Cerda 5037 / Santiago-Cerrillos');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','3100001044','Exequiel Fernandez 3305 Macul');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','3100000990','Av Bilbao 1127 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','200088','Av Salvador 1406 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','3100000984','Av Bilbao 2589 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','3100000974','Av Vicuña Mackenna 7110 local 120 La Florida');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicio Tecnico GMB','','200088','3100001475','Av Americo Vespucio 1001 Huechuraba');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000963','Prat 1099 Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000921','Collin 833 Chillan');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000923','OHiggins 1137 Chillan');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100001032','Ruta 5 Sur Km 508 Los Angeles');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','232249','21 de Mayo 3225 Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000914','Av Caupolican 1476 Temuco');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000925','Hoschtetter 946 Temuco');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000927','Camilo Henriquez 610 Valdivia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servicios Cordillera','','237622','3100000929','Av Jorge Alessandri 885 Hualpen Concepcion');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Servital','','232730','232730','Vicuña Mackenna 2545 San Joaquin Santiago');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Siglo XXI','','232199','3100000924','Av Argentina 1130 Los Andes');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Siglo XXI','','232199','232199','Bellavista 0170 Providencia');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Siglo XXI','','232199','3100001573','Santo Domingo 195 San Felipe');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('SKBerge Logistica SA','','IBC15','IBC15','Camino a Noviciado Km 3');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Sociedad Hermanas Callegari','','200100','3100000913','Ramon Freire  210 Copiapo');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Sociedad Hermanas Callegari','','200100','3100000911','OHiggins 401 Copiapo');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Sociedad Hermanas Callegari','','200100','200100','Av Francisco de Aguirre 060 La Serena');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Sociedad Hermanas Callegari','','200100','3100000918','Covarrubias 340 Ovalle');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Varona Coyhaique','','232093','232093','Carrera 333 Coyhaique');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Vega Artus ','','201016','3100000945','Av Miguel Ramirez 199 Rancagua');
INSERT INTO sucursal(nombreConcesionario,nombreComuna,numeroFactura,ShipCode,direccionSucursal)VALUES('Vega Artus ','','201016','201016','Miguel Ramirez 230 Rancagua');

--SUCURSAL PARA COTIZAR
INSERT INTO sucursal(nombreComuna,nombreConcesionario,numeroFactura,shipCode,direccionSucursal)VALUES('Santiago','cotizadores','sta. teresita #4456','sta. teresita #4456','');
--SUCURSAL PARA COTIZAR


--CASA CENTRAL
INSERT INTO sucursal(nombreComuna,nombreConcesionario,numeroFactura,shipCode,direccionSucursal)VALUES('Santiago','SKBERGE','SKBERGE','X','X')
--CASA CENTRAL

--USUARIOS

	INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('X',17707464,'Rodrigo','administrador',ENCRYPTBYPASSPHRASE('ENCRIPTADO','administrador'),'rodrigo.rivero@intellicore.cl','02-1234567',0,1);	
	INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('3100000943',11111111,'Jorge','concesionarioG',ENCRYPTBYPASSPHRASE('ENCRIPTADO','concesionarioG'),'jorge@hotmail.com','02-1234567',0,1);
	INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('3100000943',22222222,'Charlie','concesionarioO',ENCRYPTBYPASSPHRASE('ENCRIPTADO','concesionarioO'),'charlie@hotmail.com','02-1234567',0,1);
	INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('sta. teresita #4456',33333333,'José','usuarioComun',ENCRYPTBYPASSPHRASE('ENCRIPTADO','usuarioComun'),'jose@hotmail.com','02-1234567',0,1);
	--AUOFRANCE
		INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('232661',44444444,'Charlie','concesionarioO1',ENCRYPTBYPASSPHRASE('ENCRIPTADO','concesionarioO'),'charlie@hotmail.com','02-1234567',0,1);
		INSERT INTO persona(shipCode,rut,nombre,usuario,contrasena,email,telefono,multipleSucursal,habilitado) values('232661',55555555,'José','usuarioComun1',ENCRYPTBYPASSPHRASE('ENCRIPTADO','usuarioComun'),'jose@hotmail.com','02-1234567',0,1);
	--AUOFRANCE
--USUARIOS

--PERMISOS
INSERT INTO personaPermisos(rut,cargo,descripcion)values(17707464,1,'administrador,administrador,administra todo el sitio, el administrador se encuentra en casa central skberge');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(11111111,2,'concesionario gerente,concesionario gerente,es el concesionario encargado de monitorear a sus empleados, se encuentra en algun concesionario, SOLO HAY UNO POR CONCESIONARIO');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(22222222,3,'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios');
INSERT INTO personaPermisos(rut,cargo,descripcion)values(33333333,4,'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio');
--PERMISOS
	--AUOFRANCE
		INSERT INTO personaPermisos(rut,cargo,descripcion)values(44444444,3,'concesionario operario,concesionario operario,es el "comprador", es el, el encargado de realizar los pedidos, se encuentran en los concesionarios');
		INSERT INTO personaPermisos(rut,cargo,descripcion)values(55555555,3,'usuario comun/cotizador,es cualquier usuario que se le otorgue una password y usuario para poder navegar por el sitio');
	--AUOFRANCE


--MARCAS
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('SSANGYONG','SSY','marca de auto','BC08');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('MITSUBISHI','MMC','marca de auto','BC04');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('FIAT','IT','marca de auto','BC03');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('CHRYSLER','CH','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('RAM','RM','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('CHERY','NW','marca de auto','BC10');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('MG','MG','marca de auto','BC11');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('TATA','TATA','marca de auto','BC13');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('JEEP','JP','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('DODGE','DG','marca de auto','BC02');
INSERT INTO marca(nombreMarca,abreviado,descripcion,orgVentas)VALUES('ALFA ROMEO','AR','marca de auto','BC03');
--MARCAS

--MARCAS SEGUN CONCESIONARIO
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('APC Servicios','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('APC Servicios','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('APC Servicios','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('APC Servicios','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autofrance','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autofrance','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autoimpacto Chillan','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotora Goma','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotriz Fernando Korner','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotriz Val','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotriz Val','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Automotriz Val','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit Stgo','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit Stgo','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit Stgo','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Autosummit Stgo','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Aventura Motors ','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Aventura Motors ','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Aventura Motors ','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Aventura Motors ','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Bruno Fritsch','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Callegari e Hijos La Serena','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Cartoni','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Cartoni','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Cartoni','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Casa Matriz','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('COMASA PALD','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Santa Isabel','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comasa Vitacura','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Automotora Prime','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Automotora Prime','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Comercial Rosselot','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Difor','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Ditalcar','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Ditalcar','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Ditalcar','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Emprenani','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Expoautos','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Expoautos','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Felipe Noguera ','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Hernandez Motores ','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Hernandez Motores ','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Jaime Navarrete Garcia','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Jorge Osorio Uribe','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Jose Eterovic','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Jose Vergara','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('La Foresta ','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marcelo Fronza y Cia Ltda','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marcelo Fronza y Cia Ltda','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marcelo Fronza y Cia Ltda','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marcelo Fronza y Cia Ltda','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marco Ratti ','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marco Ratti ','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marco Ratti ','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Marco Ratti ','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Nallar Autos Osorno','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Noack','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Noack','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patagonia Automotriz','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Patricio Gracia ','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Piamonte','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Piamonte','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Piamonte','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Pinturas Automotrices P Montt','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rental ','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rental ','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rental ','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rental ','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rental ','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express Viña','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express Viña','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express Viña','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express Viña','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Repuestos Express Viña','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rodar','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Rodar','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKCOMERCIAL SA','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Automotrices Integrales Ltda','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicio Tecnico GMB','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servicios Cordillera','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servital','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servital','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Servital','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Siglo XXI','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Siglo XXI','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Siglo XXI','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Siglo XXI','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('SKBerge Logistica SA','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Sociedad Hermanas Callegari','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Sociedad Hermanas Callegari','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Sociedad Hermanas Callegari','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Sociedad Hermanas Callegari','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Sociedad Hermanas Callegari','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Transworld Punta Arenas','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','SSANGYONG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','MITSUBISHI');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','FIAT');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','RAM');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','CHERY');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','MG');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','TATA');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','JEEP');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','DODGE');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Varona Coyhaique','ALFA ROMEO');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Vega Artus ','CHRYSLER');
INSERT INTO concesionarioMarca(nombreConcesionario,nombreMarca)VALUES('Vega Artus ','FIAT');


--SKBERGE CASA MATRIZ (Todas LAS MARCAS)

--COTIZADORES POSEE Todas LAS MARCAS
INSERT INTO concesionarioMarca VALUES('ALFA ROMEO','cotizadores');
INSERT INTO concesionarioMarca VALUES('CHERY','cotizadores');
INSERT INTO concesionarioMarca VALUES('CHRYSLER','cotizadores');
INSERT INTO concesionarioMarca VALUES('DODGE','cotizadores');
INSERT INTO concesionarioMarca VALUES('FIAT','cotizadores');
INSERT INTO concesionarioMarca VALUES('JEEP','cotizadores');
INSERT INTO concesionarioMarca VALUES('MG','cotizadores');
INSERT INTO concesionarioMarca VALUES('MITSUBISHI','cotizadores');
INSERT INTO concesionarioMarca VALUES('SSANGYONG','cotizadores');
INSERT INTO concesionarioMarca VALUES('RAM','cotizadores');
--COTIZADORES POSEE Todas LAS MARCAS

--MODELO
	--JEEP
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Wrangler','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Patriot ','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Compass','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Cherokee','Modelo de la marca');		
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Grand Cherokee','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('JEEP','Todas','Modelo de la marca');
	--JEEP
	
	--CHRYSLER
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHRYSLER','Town & Country','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHRYSLER','Todas','Modelo de la marca');
	--CHRYSLER
	
	--DODGE
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Durango','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Journey','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Nitro','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Caliber','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Dakota','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Challenger','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('DODGE','Todas','Modelo de la marca');
	--DODGE
	
	--RAM
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('RAM','1500','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('RAM','2500','Modelo de la marca');		
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('RAM','Todas','Modelo de la marca');		
	--RAM
	
	--TATA
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('TATA','Xenon','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('TATA','Todas','Modelo de la marca');
	--TATA
	
	--FIAT
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
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('FIAT','Todas','Modelo de la marca');
	--FIAT
	
	--MITSUBISHI
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
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MITSUBISHI','Todas','Modelo de la marca');
	--MITSUBISHI
	
	--SSANGYONG
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Actyon Sport','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Rexton','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Korando','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Kyron','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Actyon ','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Stavic','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('SSANGYONG','Todas','Modelo de la marca');

	--SSANGYONG
	
	--CHERY
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','IQ','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Face','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Skin','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Tiggo','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Destiny','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Beat','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('CHERY','Todas','Modelo de la marca');
	--CHERY
	
	--ALFA ROMEO
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Giulietta','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','159','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Brera','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Mito','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('ALFA ROMEO','Todas','Modelo de la marca');
	--ALFA ROMEO
	
	--MG
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','MG-350','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','MG-550','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','MG-750','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','MG-6','Modelo de la marca');
		INSERT INTO modelo(nombreMarca,nombreModelo,descripcion)VALUES('MG','Todas','Modelo de la marca');
	--MG
--MODELO

--GRUPOMATERIALES
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBA','BC03','FIAT','IT','ITR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBB','BC03','ALFA ROMEO','IT','ITR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBC','BC03','LANCIA','IT','ITR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBD','BC03','FERRARI','IT','ITR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBE','BC03','MASSERATI','IT','ITR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBF','BC04','MITSUBISHI','MM','MMR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBG','BC04','FUSO','MM','MMR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBH','BC10','CHERY','NW','NWR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBI','BC02','JEEP','CH','CHR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBJ','BC02','CHRYSLER','CH','CHR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBK','BC02','DODGE','CH','CHR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBL','BC08','SSANGYONG','SS','SSR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBM','BC11','MG','SP','SPR');
	INSERT INTO GRUPO_MATERIALES(GRUPO_MATERIAL,VKORG,MARCA,CMPY_CODE,PREFIJO)VALUES('ZBN','BC13','TATA','TA','TAR');
--GRUPOMATERIALES


--Tablas de paso para, resultado de busquedas y cadenas de reemplazo
CREATE TABLE LISTA_BUSQUEDA_TMP(
	ID_SESSION VARCHAR(100) NOT NULL,
	MARCA VARCHAR(100) NOT NULL,
	CANTIDAD VARCHAR(50) NOT NULL,
	PRECIO_LISTA VARCHAR(50) NOT NULL,
	PRECION_CONCE VARCHAR(50) NOT NULL,
	GRUPO_MAT VARCHAR(20) NOT NULL,
	DESCRIP VARCHAR(200) NOT NULL,
	CODIGO VARCHAR(50) NOT NULL,
	STOCK VARCHAR(50) NOT NULL,
	TOTAL_C VARCHAR(50) NOT NULL,
	TOTAL_L VARCHAR(50) NOT NULL
)

CREATE TABLE LISTA_REEMPLAZO_TMP(
	ID_SESSION VARCHAR(100) NOT NULL,
	MARCA VARCHAR(100) NOT NULL,
	CANTIDAD VARCHAR(50) NOT NULL,
	T_INTTYPE VARCHAR(50) NOT NULL,
	T_KBETR1 VARCHAR(50) NOT NULL,
	T_KBETR2 VARCHAR(50) NOT NULL,
	T_KONDM VARCHAR(20) NOT NULL,
	T_MAKTX VARCHAR(200) NOT NULL,
	T_MFRPN VARCHAR(50) NOT NULL,
	TOTAL_C VARCHAR(50) NOT NULL,
	TOTAL_L VARCHAR(50) NOT NULL,
	STOCK VARCHAR(50) NOT NULL	
);
GO

--PROCEDIMIENTO QUE CADUCA LAS COTIZACIONES
CREATE PROCEDURE SP_CADUCAR_COTIZACION (@NUM_PEDIDO VARCHAR(20))
AS
BEGIN
	DECLARE @FECHA_EXP INT;
	DECLARE @FECHA_ACTUAL INT;
	DECLARE @FECHA_EXP_SYS INT;
	
	SET @FECHA_EXP = (SELECT cast(FECHA_EXPIRACION AS INT) FROM PEDIDO WHERE E_VBELN = @NUM_PEDIDO);
	SET @FECHA_EXP_SYS = (select CAST(FECHA_EXPIRACION_SYS AS INT) FROM pedido WHERE E_VBELN = @NUM_PEDIDO);
	SET @FECHA_ACTUAL = (select CAST(convert(datetime,CONVERT(varchar(10), GETDATE(), 103),103)  AS INT));
	
	
	IF @FECHA_EXP = @FECHA_ACTUAL
	BEGIN
		UPDATE PEDIDO SET estado = 'CADUCADO_SAP' WHERE E_VBELN = @NUM_PEDIDO;
		--PRINT 'WENAA';
	END
	ELSE IF @FECHA_EXP_SYS = @FECHA_ACTUAL
	BEGIN
		UPDATE PEDIDO SET estado = 'CADUCADO_SYS' WHERE E_VBELN = @NUM_PEDIDO;
		--PRINT 'WENAA 2';
	END
	--ELSE --PRINT 'MALA';

END 
GO
--DROP TABLE LISTA_REEMPLAZO_TMP
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




