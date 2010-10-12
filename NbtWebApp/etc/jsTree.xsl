<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:include href="lib.xsl" />

<xsl:output method="xml" omit-xml-declaration="yes"/>

<xsl:variable name="newline">
<xsl:text>
</xsl:text>
</xsl:variable>


<xsl:template match="NbtTree">
  <xsl:element name="root" >
    <xsl:call-template name="Loop">
    </xsl:call-template>
  </xsl:element>
</xsl:template>


<xsl:template name="Loop">
  <xsl:for-each select="NbtNodeGroup">
    <xsl:element name="item">
      <xsl:attribute name="id">
        <xsl:value-of select="@key" />
      </xsl:attribute>
      <xsl:attribute name="rel">
        <xsl:text>group</xsl:text>
      </xsl:attribute>
      <xsl:element name="content">
        <xsl:element name="name">
          <xsl:value-of select="@name" />
        </xsl:element>
      </xsl:element>
      <xsl:text>
      </xsl:text>
      <xsl:call-template name="Loop">
      </xsl:call-template>
    </xsl:element>
  </xsl:for-each>

  <xsl:for-each select="NbtNode">
    <xsl:choose>
      <xsl:when test="@showintree='true'">
        <xsl:element name="item">
          <xsl:attribute name="id">
            <xsl:value-of select="@key" />
          </xsl:attribute>
          <xsl:choose>
            <xsl:when test="@nodetypeid=0">
              <xsl:attribute name="state">
                <xsl:text>open</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="rel">
                <xsl:text>nodetypeid_0</xsl:text>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="state">
                <xsl:text>closed</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="rel">
                <xsl:text>nodetypeid_</xsl:text>
                <xsl:value-of select="@nodetypeid" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:element name="content">
            <xsl:element name="name">
              <xsl:value-of select="@nodename" />
            </xsl:element>
          </xsl:element>
          <xsl:text>
          </xsl:text>
          <xsl:call-template name="Loop">
          </xsl:call-template>
        </xsl:element>
      </xsl:when>

      <xsl:otherwise>
        <xsl:call-template name="Loop">
        </xsl:call-template>
      </xsl:otherwise>
      
    </xsl:choose>
  </xsl:for-each>

</xsl:template>


<xsl:template match="/">
        <xsl:apply-templates/>
</xsl:template>

</xsl:stylesheet>