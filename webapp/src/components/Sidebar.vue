<template>
    <div id="sidebar-collapse" class="col-sm-3 col-lg-2 sidebar">
      <ul class="nav menu">
        <router-link tag="li" to="/" exact><a>Painel</a></router-link>
        <router-link tag="li" to="/customers"><a>Clientes</a>
        </router-link>
        <router-link tag="li" to="/leads"><a>Oportunidades</a></router-link>
        <router-link tag="li" to="/activities"><a>Atividades</a></router-link>
        <router-link tag="li" to="/salesorders"><a>Encomendas</a></router-link>
      </ul>
    </div>
</template>

<script>
export default {
  name: 'sidebar',
  data () {
    return {}
  }
}
</script>
