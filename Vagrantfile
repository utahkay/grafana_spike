# -*- mode: ruby -*-
# vi: set ft=ruby :

#ENV['VAGRANT_DEFAULT_PROVIDER'] = 'docker'

Vagrant.configure(2) do |config|
  config.vm.provider "docker" do |docker|
    docker.vagrant_vagrantfile = "vagrant-host/Vagrantfile"
  end

  config.vm.define "grafana" do |container|
    container.vm.synced_folder ".", "/vagrant", disabled: true
    container.vm.provider "docker" do |docker|
      docker.image = "grafana/grafana"
      docker.ports = ['8800:3000']
      docker.name = 'grafana'
    end
  end

  
  config.vm.define "graphite" do |container|
    container.vm.synced_folder ".", "/vagrant", disabled: true
    container.vm.provider "docker" do |docker|
      docker.image = "hopsoft/graphite-statsd"
      docker.ports = ['8801:80', '2003:2003', '8125:8125/udp']
      docker.name = 'graphite'
	  end
#    container.vm.network "forwarded_port", guest: 8801, host: 80
#    container.vm.network "forwarded_port", guest: 2003, host: 2003
#    container.vm.network "forwarded_port", guest: 8125, host: 8125
  end
  
end
