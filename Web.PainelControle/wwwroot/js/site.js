// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const tipoCadsatroAutomatico = 1
    , tipoCadsatroManual     = 2

/**
 * Função para carregar WSDL e arquivos XSD
 * @param {string} wsdlUrl - URL do arquivo WSDL principal
 * @returns {Promise<Array<{nome: string, conteudo: string}>>}
 */
async function carregarWsdlEXsd(wsdlUrl) {
  const resultados = [];

  // Buscar WSDL principal
  const wsdlConteudo = await fetchArquivo(wsdlUrl);
  if (wsdlConteudo) {
    resultados.push({ nome: wsdlUrl, conteudo: wsdlConteudo });
  }

  /*
  // Buscar todos os XSDs
  for (const xsdUrl of xsdUrls) {
    const xsdConteudo = await fetchArquivo(xsdUrl);
    if (xsdConteudo) {
      resultados.push({ nome: xsdUrl, conteudo: xsdConteudo });
    }
  }
  */

  return resultados;
}

// Função auxiliar para buscar conteúdo
async function fetchArquivo(url) {
  try {
    const response = await fetch(url);
    if (!response.ok) {
      throw new Error(`Erro ao buscar ${url}: ${response.statusText}`);
    }
    const texto = await response.text();
    return texto;
  } catch (error) {
    console.error(error);
    return null;
  }
}