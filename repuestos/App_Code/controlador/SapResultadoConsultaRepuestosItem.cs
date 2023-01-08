using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de SapResultadoConsultaRepuestosItem
/// </summary>
public class SapResultadoConsultaRepuestosItem
{

        private string _DenominacionField;
        private string _NumPiezaFabricanteField; //EZ_MFRPN
        private string _DescripcionField; //EZ_MAKTX
        private string _GrupoMaterialesField; //EZ_KONDM
        private string _StockField;
        private string _ValorField;



        public string Denominacion
        {
            get
            {
                return this._DenominacionField;
            }
            set
            {
                this._DenominacionField = value;
            }

        }

        public string NumPiezaFabricante
        {
            get
            {
                return this._NumPiezaFabricanteField;
            }
            set
            {
                this._NumPiezaFabricanteField = value;
            }
        }

        public string Descripcion
        {
            get
            {
                return this._DescripcionField;
            }
            set
            {
                this._DescripcionField = value;
            }
        }

        public string GrupoMateriales
        {
            get
            {
                return this._GrupoMaterialesField;
            }
            set
            {
                this._GrupoMaterialesField = value;
            }
        }

        public string Stock
        {
            get
            {
                return this._StockField;
            }
            set
            {
                this._StockField = value;
            }
        }


        public string Valor
        {
            get
            {
                return this._ValorField;
            }
            set
            {
                this._ValorField = value;
            }
        }


    
}