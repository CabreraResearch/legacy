<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="lib.xsl" />

  <xsl:output method="xml" omit-xml-declaration="yes"/>

  <xsl:variable name="newline">
  <xsl:text>
  </xsl:text>
  </xsl:variable>


  <xsl:template match="NbtTree">
    <xsl:element name="Nodes" >
      <xsl:value-of select="$newline"/>
      <xsl:call-template name="Loop">
      </xsl:call-template>
      <xsl:value-of select="$newline"/>
    </xsl:element>
  </xsl:template>


  <xsl:template name="Loop">
    <xsl:for-each select="NbtNode">      

      <xsl:element name="Node">
        <xsl:attribute name="nodename"><xsl:value-of select="@nodename"/></xsl:attribute>
        <xsl:attribute name="nodeid"><xsl:value-of select="@nodeid"/></xsl:attribute>
        <xsl:attribute name="nodetypeid"><xsl:value-of select="@nodetypeid"/></xsl:attribute>
        <xsl:value-of select="$newline"/>
        <xsl:call-template name="PropertyLoop">
        </xsl:call-template>
        <xsl:value-of select="$newline"/>
        <xsl:call-template name="Loop">
        </xsl:call-template>
      </xsl:element>

    </xsl:for-each>
  </xsl:template>

  <xsl:template name="PropertyLoop">
    <xsl:for-each select="NbtNodeProp">
      <xsl:element name="PropValue">
        <xsl:attribute name="propid"><xsl:value-of select="@nodetypepropid"/></xsl:attribute>
        <xsl:attribute name="stringvalue"><xsl:value-of select="@gestalt"/></xsl:attribute>
      </xsl:element>
      <xsl:value-of select="$newline"/>
    </xsl:for-each>
  </xsl:template>

    
  <xsl:template match="/">
          <xsl:apply-templates/>
  </xsl:template>

</xsl:stylesheet>