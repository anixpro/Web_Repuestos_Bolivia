-- declaramos las variables
declare @nombre as varchar(50)
declare @rut as varchar(50)
DECLARE @largo  int;
-- declaramos un cursor llamado "CURSORITO".El select debe contener sólo los campos a utilizar.
declare CURSORITO cursor for
select nombre, rut from persona
open CURSORITO
-- Avanzamos un registro y cargamos en las variables los valores encontrados en el primer registro
fetch next from CURSORITO
into @nombre, @rut
    while @@fetch_status = 0
        begin
        SET @largo =LEN(@rut)-3
        update PERSONA set contrasena=ENCRYPTBYPASSPHRASE('ENCRIPTADO',SUBSTRING(@nombre,0,5)+SUBSTRING(CAST(@rut as VARCHAR),@largo,4)),usuario=@rut where rut=@rut
        --update Cliente set CliPass= @user where CliCod=@cod
        -- Avanzamos otro registro
        fetch next from CURSORITO
        into @nombre, @rut
        end
-- cerramos el cursor
close CURSORITO
deallocate CURSORITO


