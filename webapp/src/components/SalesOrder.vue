<template>
  <div class="col-sm-9 col-sm-offset-3 col-lg-10 col-lg-offset-2 main">
    <breadcrumb :items="[{path:'salesorders',name:'Encomendas'}]" :current="'Encomenda'"></breadcrumb>

    <div class="row">
      <div class="col-lg-12">
        <h1 class="page-header">Detalhes Encomenda</h1>
      </div>
    </div>
    <!--/.row-->

    <div class="row">
      <div class="col-lg-8">
        <div class="panel panel-default">
          <div class="panel-heading">Info
            <a :href='"http://localhost:49559/api/encomendas/" + encomenda.idInterno + "/pdf"' target="_blank">
              <img class="pull-right" src="/static/images/pdf.png" height="100%" alt="Gerar PDF"/>
            </a>
          </div>
          <div class="panel-body">
            <form class="form-horizontal">
              <div class="form-group">
                <label for="numdoc" class="col-sm-2 control-label">Número Documento</label>
                <div class="col-sm-10">
                  <input type="text" class="form-control" id="numdoc" placeholder="Id" v-model="encomenda.NumeroDocumento" disabled>
                </div>
              </div>

              <div class="form-group">
                <label for="data" class="col-sm-2 control-label">Data</label>
                <div class="col-sm-10">
                  <input type="date" class="form-control" id="data" placeholder="Data" v-model="encomenda.Data"  disabled>
                </div>
              </div>

              <div class="form-group">
                <label for="cliente" class="col-sm-2 control-label">Cliente</label>
                <div class="col-sm-10">
                  <input type="text" class="form-control" id="cliente" placeholder="Cliente" v-model="encomenda.Entidade"  disabled>
                </div>
              </div>

              <div class="form-group">
                <label for="oportunidade" class="col-sm-2 control-label">Oportunidade</label>
                <div class="col-sm-10">
                  <input type="text" class="form-control" id="oportunidade" placeholder="Oportunidade" v-model="encomenda.oportunidade"  disabled>
                </div>
              </div>

              <div class="form-group">
                <label for="serie" class="col-sm-2 control-label">Série</label>
                <div class="col-sm-10">
                  <input type="text" class="form-control" id="serie" placeholder="Série" v-model="encomenda.Serie"  disabled>
                </div>
              </div>

              <div class="form-group">
                <label for="valor" class="col-sm-2 control-label">Total Mercadoria</label>
                <div class="col-sm-10">
                  <input type="text" class="form-control" id="valor" placeholder="Valor" v-model="encomenda.TotalMercadoria" disabled>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>

    <article-listing :artigos="this.artigos" :lock=true></article-listing>
  </div>
</template>

<script>
import config from '../assets/config.json'
import ArticleListing from './ArticleListing.vue'

export default {
  name: 'SalesOrders',
  data () {
    return {editing:false,encomenda:{},artigos:[]}
  },
  components:{ArticleListing},
  methods:{
  },
  mounted:function(){
    this.$http.get(encodeURI(config.host+'/api/encomendas/'+this.$route.params.id))
    .then((response)=>{
      this.encomenda=response.body;
      console.log(this.encomenda);
      let d = new Date(this.encomenda.Data);
      this.encomenda.Data=d.getUTCFullYear()  +'-'+("0" + (d.getMonth() + 1)).slice(-2) + '-' + ("0" + d.getDate()).slice(-2);
      this.artigos=response.body.LinhasDocumento;
      if(this.encomenda.idOportunidade){
        this.$http.get(config.host+'/api/oportunidade/'+encomenda.idOportunidade)
        .then((response)=>{
          this.encomenda.oportunidade=response.body.descricao;
        })
      }else{
        this.encomenda.oportunidade="Não existe oportunidade de venda associada"
      }

    });
  }
}
</script>

<style>
.recipt {
  margin-top: 10px;
}
</style>
