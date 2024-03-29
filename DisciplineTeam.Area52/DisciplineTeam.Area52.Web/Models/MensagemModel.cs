﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DisciplineTeam.Area52.Web.Models
{
    public class MensagemModel : ModelBase
    {
        //Post da mensagem
        public void PostMensagem(Mensagem e, int iduser, int idgrupo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"EXEC cadMsg @texto, @usuario_id, @grupo_id";

            cmd.Parameters.AddWithValue("@texto", e.Texto);
            cmd.Parameters.AddWithValue("@usuario_id", iduser);
            cmd.Parameters.AddWithValue("@grupo_id", idgrupo); 

            cmd.ExecuteNonQuery();
        }
        //Leitura das Mensagens dentro do grupo
        public List<ViewAll> ReadMensagem(int idgrupo, int quant)
        {
            List<ViewAll> lista = new List<ViewAll>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandText = @"SELECT TOP " + quant + " * FROM v_Grupo_Msg WHERE @idgrupo = grupo_id ORDER BY Datahora DESC";

            cmd.Parameters.AddWithValue("@idgrupo", idgrupo);
            //cmd.CommandType = System.Data.CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ViewAll p = new ViewAll();
                p.MsgIdMensagem = (int)reader["MsgId"];
                DateTime data = (DateTime)reader["Datahora"];
                p.MsgDatahora = data.ToString("dd/MM/yyyy, HH:mm");
                p.MsgTexto = (string)(reader["Texto"]);
                p.PIdPessoa = (int)reader["IdUsuario"];
                p.UNick = (string)reader["Nickusuario"];
                p.UImagem = (string)reader["Imagemusuario"];
                p.GIdGrupo = (int)reader["Idgrupo"];
                p.GNome = (string)reader["Nomegrupo"];
                p.PartStatus = (int)reader["PartStatus"];
                lista.Add(p);
            }
            return lista;
        }
        //Retorna as ultimas 10 mensagens postadas ordenadas por data
        public List<ViewAll> ReadMensagemIndex(int iduser, int quant)
        {
            List<ViewAll> lista = new List<ViewAll>();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;

            cmd.CommandText = @"SELECT TOP " + quant + " * FROM v_Grupo_Msg_Part WHERE PartIdUser = @iduser AND PartIdGrupo = Idgrupo AND (PartStatus = 1 OR PartStatus = 2) ORDER BY Datahora DESC";
                   

            cmd.Parameters.AddWithValue("@iduser", iduser);
            //cmd.CommandType = System.Data.CommandType.Text;

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ViewAll p = new ViewAll();
                DateTime data = (DateTime)reader["Datahora"];
                p.MsgDatahora = data.ToString("dd/MM/yyyy, HH:mm");
                p.MsgTexto = (string)(reader["Texto"]);
                p.PIdPessoa = (int)reader["IdUsuario"];
                p.UNick = (string)reader["Nickusuario"];
                p.UImagem = (string)reader["Imagemusuario"];
                p.GIdGrupo = (int)reader["Idgrupo"];
                p.GNome = (string)reader["Nomegrupo"];

                lista.Add(p);
            }
            return lista;
        }
        //Retorna a quantidade de mensagens do usuario para mostrar o load de mensagens
        public int QuantMsgUser(int iduser)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"SELECT COUNT (*) FROM v_Grupo_Msg_Part WHERE PartIdUser = @iduser AND PartIdGrupo = Idgrupo AND (PartStatus = 1 OR PartStatus = 2)";

            cmd.Parameters.AddWithValue("@iduser", iduser);

            int quant = (int)cmd.ExecuteScalar();
            return quant;
        }
        //Retorna a quantidade de mensagens do grupo para mostrar o load de mensagens
        public int QuantMsgGrupo(int idgrupo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"SELECT COUNT (*) FROM v_Grupo_Msg WHERE @idgrupo = grupo_id";

            cmd.Parameters.AddWithValue("@idgrupo", idgrupo);

            int quant = (int)cmd.ExecuteScalar();
            return quant;
        }
        public void DeleteMsgUser(int iduser, int idgrupo, int idmsg)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = @"DELETE FROM mensagens WHERE usuario_id = @iduser AND grupo_id = @idgrupo AND id = @idmsg";

            cmd.Parameters.AddWithValue("@iduser", iduser);
            cmd.Parameters.AddWithValue("@idgrupo", idgrupo);
            cmd.Parameters.AddWithValue("@idmsg", idmsg);

            cmd.ExecuteNonQuery();
        }
    }
}